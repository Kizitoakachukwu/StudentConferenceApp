using Microsoft.AspNetCore.Hosting;
using System.IO.Compression;
using System.Globalization;
using ClosedXML.Excel;
using DataAccess.Entities;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using StudentConferenceApp.BLL.DTOs;
using StudentConferenceApp.BLL.Interfaces;
using StudentConferenceApp.DAL.Repositories.Interfaces;

namespace StudentConferenceApp.BLL.Services;

public class DocumentService : IDocumentService
{
    private readonly IParticipantRepository _participants;
    private readonly IWebHostEnvironment _env;

    static DocumentService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public DocumentService(IParticipantRepository participants, IWebHostEnvironment env)
    {
        _participants = participants;
        _env = env;
    }

    public Task<string> GenerateInvitationLetterAsync(ParticipantDto participant)
    {
        var relative = $"/generated/invitations/Invitation_{participant.Id}.txt";
        var dir = Path.Combine(_env.WebRootPath, "generated", "invitations");
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, $"Invitation_{participant.Id}.txt");
        File.WriteAllText(path, $"Invitation placeholder for {participant.FullName}.");
        return Task.FromResult(relative);
    }

    public Task<string> GenerateCertificateAsync(ParticipantDto participant)
    {
        var relative = $"/generated/certificates/Certificate_{participant.Id}.txt";
        var dir = Path.Combine(_env.WebRootPath, "generated", "certificates");
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, $"Certificate_{participant.Id}.txt");
        File.WriteAllText(path, $"Certificate placeholder for {participant.FullName}.");
        return Task.FromResult(relative);
    }

    public Task<string> GenerateParticipantListAsync(List<ParticipantDto> participants)
    {
        var relative = $"/generated/reports/ParticipantList_{DateTime.UtcNow:yyyyMMdd}.txt";
        var dir = Path.Combine(_env.WebRootPath, "generated", "reports");
        Directory.CreateDirectory(dir);
        var physical = Path.Combine(dir, $"ParticipantList_{DateTime.UtcNow:yyyyMMdd}.txt");
        File.WriteAllText(physical, string.Join(Environment.NewLine, participants.Select(p => p.FullName)));
        return Task.FromResult(relative);
    }

    public Task<bool> SendConfirmationEmailAsync(ParticipantDto participant) => Task.FromResult(true);

    public async Task<string> GenerateRegistrationDataReportAsync(int participantId)
    {
        var p = await _participants.GetByIdWithManagerAsync(participantId)
            ?? throw new InvalidOperationException("Participant not found.");

        var folder = Path.Combine(_env.WebRootPath, "generated", "registration");
        Directory.CreateDirectory(folder);
        var stamp = $"{participantId}_{DateTime.UtcNow:yyyyMMddHHmmss}";
        var docxPath = Path.Combine(folder, $"{stamp}_reg.docx");
        var xlsxPath = Path.Combine(folder, $"{stamp}_reg.xlsx");
        var zipPath = Path.Combine(folder, $"{stamp}.zip");

        WriteRegistrationWord(docxPath, p);
        WriteRegistrationExcel(xlsxPath, p);

        if (File.Exists(zipPath))
            File.Delete(zipPath);

        using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
        {
            zip.CreateEntryFromFile(docxPath, "RegistrationData.docx", CompressionLevel.Optimal);
            zip.CreateEntryFromFile(xlsxPath, "RegistrationData.xlsx", CompressionLevel.Optimal);
        }

        File.Delete(docxPath);
        File.Delete(xlsxPath);

        return $"/generated/registration/{Path.GetFileName(zipPath)}";
    }

    public async Task<string> GenerateFinalReportAsync()
    {
        var list = (await _participants.GetAllParticipantsWithDetailsAsync()).ToList();
        var folder = Path.Combine(_env.WebRootPath, "generated", "final");
        Directory.CreateDirectory(folder);
        var fileName = $"FinalReport_{DateTime.UtcNow:yyyyMMddHHmmss}.docx";
        var path = Path.Combine(folder, fileName);

        using var wordDoc = WordprocessingDocument.Create(path, DocumentFormat.OpenXml.WordprocessingDocumentType.Document);
        var main = wordDoc.AddMainDocumentPart();
        main.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
        var body = main.Document.AppendChild(new Body());

        var orderedSections = list
            .GroupBy(x => new { x.SectionId, x.SectionName })
            .OrderBy(g => g.Key.SectionId ?? int.MaxValue)
            .ThenBy(g => g.Key.SectionName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var sectionIndex = 0;
        foreach (var group in orderedSections)
        {
            sectionIndex++;
            var sectionTitle = string.IsNullOrWhiteSpace(group.Key.SectionName) ? "UNASSIGNED" : group.Key.SectionName.Trim();
            body.AppendChild(FinalReportSectionTitleParagraph(sectionIndex, sectionTitle));
            body.AppendChild(FinalReportBlankParagraph());

            var seq = 0;
            foreach (var p in group.OrderBy(x => x.FullName, StringComparer.OrdinalIgnoreCase).ThenBy(x => x.Id))
            {
                seq++;
                body.AppendChild(FinalReportPaperTitleParagraph(seq, p));
                body.AppendChild(FinalReportSpeakerParagraph(p.FullName));
                var supervisorPara = FinalReportSupervisorParagraph(p.Manager);
                if (supervisorPara != null)
                    body.AppendChild(supervisorPara);
                body.AppendChild(FinalReportBlankParagraph());
            }
        }

        body.AppendChild(new SectionProperties());
        main.Document.Save();

        return $"/generated/final/{fileName}";
    }

    public async Task<string> GenerateFinalReportPdfAsync()
    {
        var list = (await _participants.GetAllParticipantsWithDetailsAsync()).ToList();
        var folder = Path.Combine(_env.WebRootPath, "generated", "final");
        Directory.CreateDirectory(folder);
        var fileName = $"FinalReport_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
        var path = Path.Combine(folder, fileName);

        var bytes = BuildFinalReportPdfBytes(list);
        await File.WriteAllBytesAsync(path, bytes);

        return $"/generated/final/{fileName}";
    }

    public Task<byte[]> GenerateParticipantRosterWordAsync(IReadOnlyList<ParticipantDto> participants)
    {
        using var ms = new MemoryStream();
        using (var wordDoc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
        {
            var main = wordDoc.AddMainDocumentPart();
            main.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
            var body = main.Document.AppendChild(new Body());

            body.AppendChild(new Paragraph(new Run(new RunProperties(new Bold()), new Text("Conference participants"))));
            body.AppendChild(new Paragraph(new Run(new Text($"Generated: {DateTime.UtcNow:u}"))));
            body.AppendChild(new Paragraph(new Run(new Text(" "))));

            var table = new Table();
            table.AppendChild(new TableProperties(new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }));
            table.AppendChild(CreateRosterHeaderRow());
            foreach (var p in participants.OrderBy(x => x.SectionName).ThenBy(x => x.FullName))
                table.AppendChild(CreateRosterDataRow(p));

            body.Append(table);
            body.AppendChild(new SectionProperties());
            main.Document.Save();
        }

        return Task.FromResult(ms.ToArray());
    }

    public Task<byte[]> GenerateParticipantRosterPdfAsync(IReadOnlyList<ParticipantDto> participants)
    {
        var ordered = participants.OrderBy(x => x.SectionName).ThenBy(x => x.FullName).ToList();
        using var ms = new MemoryStream();
        QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Times New Roman"));
                page.Header().Text("Conference participants").Bold().FontSize(14);
                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("Page ");
                    t.CurrentPageNumber();
                    t.Span(" / ");
                    t.TotalPages();
                });
                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(2);
                        cols.RelativeColumn(1.5f);
                        cols.RelativeColumn(1);
                        cols.RelativeColumn(1.5f);
                    });
                    table.Header(header =>
                    {
                        foreach (var h in new[] { "Speaker", "Email", "Paper", "Section", "Status", "Supervisor" })
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(4).Text(h).Bold();
                    });
                    foreach (var p in ordered)
                    {
                        table.Cell().Padding(3).Text(p.FullName);
                        table.Cell().Padding(3).Text(p.Email);
                        table.Cell().Padding(3).Text(p.PaperTitle);
                        table.Cell().Padding(3).Text(p.SectionName);
                        table.Cell().Padding(3).Text(p.Status);
                        table.Cell().Padding(3).Text(p.ManagerFullName ?? "—");
                    }
                });
            });
        }).GeneratePdf(ms);

        return Task.FromResult(ms.ToArray());
    }

    private static byte[] BuildFinalReportPdfBytes(List<Participant> list)
    {
        using var ms = new MemoryStream();
        var orderedSections = list
            .GroupBy(x => new { x.SectionId, x.SectionName })
            .OrderBy(g => g.Key.SectionId ?? int.MaxValue)
            .ThenBy(g => g.Key.SectionName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.MarginHorizontal(72);
                page.MarginVertical(72);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Times New Roman"));
                page.Content().Column(column =>
                {
                    column.Spacing(0);
                    var sectionIndex = 0;
                    foreach (var group in orderedSections)
                    {
                        sectionIndex++;
                        var sectionTitle = string.IsNullOrWhiteSpace(group.Key.SectionName)
                            ? "UNASSIGNED"
                            : group.Key.SectionName.Trim();
                        column.Item().PaddingTop(sectionIndex > 1 ? 12 : 0).AlignCenter()
                            .Text($"SECTION {sectionIndex}. {sectionTitle.ToUpperInvariant()}");
                        column.Item().Height(12);

                        var seq = 0;
                        foreach (var p in group.OrderBy(x => x.FullName, StringComparer.OrdinalIgnoreCase).ThenBy(x => x.Id))
                        {
                            seq++;
                            var titlePart = FinalReportPaperTitleWithPeriod(p.PaperTitle);
                            var org = FinalReportOrganizationParens(p);
                            var orgPart = string.IsNullOrEmpty(org) ? "" : $" ({org})";
                            column.Item().PaddingLeft(36).Text($"{seq}. {titlePart}{orgPart}");
                            column.Item().PaddingLeft(90).Text($"Speaker: {p.FullName.Trim()}").Italic();
                            if (p.Manager is { } m && !string.IsNullOrWhiteSpace(m.FullName))
                            {
                                var deg = (m.AcademicDegree ?? "").Trim();
                                var sup = string.IsNullOrEmpty(deg)
                                    ? $"Supervisor: {m.FullName.Trim()}"
                                    : $"Supervisor: {deg} {m.FullName.Trim()}";
                                column.Item().PaddingLeft(90).Text(sup).Italic();
                            }
                            column.Item().Height(12);
                        }
                    }
                });
            });
        }).GeneratePdf(ms);

        return ms.ToArray();
    }

    private static TableRow CreateRosterHeaderRow()
    {
        var row = new TableRow();
        foreach (var h in new[] { "Speaker", "Email", "Paper title", "Section", "Status", "Supervisor" })
            row.Append(CreateRosterCell(h, bold: true));
        return row;
    }

    private static TableRow CreateRosterDataRow(ParticipantDto p)
    {
        var row = new TableRow();
        row.Append(CreateRosterCell(p.FullName));
        row.Append(CreateRosterCell(p.Email));
        row.Append(CreateRosterCell(p.PaperTitle));
        row.Append(CreateRosterCell(p.SectionName));
        row.Append(CreateRosterCell(p.Status));
        row.Append(CreateRosterCell(p.ManagerFullName ?? "—"));
        return row;
    }

    private static TableCell CreateRosterCell(string text, bool bold = false)
    {
        var run = new Run(new Text(text ?? ""));
        if (bold)
            run.RunProperties = new RunProperties(new Bold());
        return new TableCell(new Paragraph(run));
    }

    /// <summary>Twentieths of a point: 1 inch = 72 pt × 20.</summary>
    private const string FirstLineIndentHalfInch = "720"; // 0.5"

    private const string FirstLineIndentOneQuarterInch = "1800"; // 1.25"

    private static ParagraphProperties FinalReportParagraphBase(JustificationValues alignment, string? firstLineTwips)
    {
        var pp = new ParagraphProperties();
        pp.Append(new Justification { Val = alignment });
        pp.Append(new SpacingBetweenLines
        {
            Before = "0",
            After = "0",
            Line = "240",
            LineRule = LineSpacingRuleValues.Auto
        });
        if (firstLineTwips != null)
            pp.Append(new Indentation { FirstLine = firstLineTwips });
        else
            pp.Append(new Indentation { FirstLine = "0" });
        return pp;
    }

    private static RunProperties FinalReportRunTnr12(bool italic)
    {
        var rp = new RunProperties();
        rp.Append(new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman", ComplexScript = "Times New Roman" });
        rp.Append(new FontSize { Val = "24" });
        rp.Append(new FontSizeComplexScript { Val = "24" });
        if (italic)
            rp.Append(new Italic(), new ItalicComplexScript());
        return rp;
    }

    private static Paragraph FinalReportSectionTitleParagraph(int sectionNumber, string sectionName)
    {
        var text = $"SECTION {sectionNumber.ToString(CultureInfo.InvariantCulture)}. {sectionName.ToUpperInvariant()}";
        var p = new Paragraph();
        p.AppendChild(FinalReportParagraphBase(JustificationValues.Center, null));
        var run = new Run();
        run.AppendChild(FinalReportRunTnr12(false));
        run.AppendChild(new Text(text));
        p.AppendChild(run);
        return p;
    }

    private static Paragraph FinalReportBlankParagraph()
    {
        var p = new Paragraph();
        p.AppendChild(FinalReportParagraphBase(JustificationValues.Both, null));
        p.AppendChild(new Run(new Text(" ") { Space = SpaceProcessingModeValues.Preserve }));
        return p;
    }

    private static string FinalReportPaperTitleWithPeriod(string paperTitle)
    {
        var t = paperTitle.Trim();
        if (string.IsNullOrEmpty(t))
            return ".";
        return t.EndsWith(".", StringComparison.Ordinal) ? t : t + ".";
    }

    private static string FinalReportOrganizationParens(Participant p)
    {
        var i = (p.Institution ?? "").Trim();
        var u = (p.University ?? "").Trim();
        if (string.IsNullOrEmpty(i))
            return string.IsNullOrEmpty(u) ? "" : u;
        if (string.IsNullOrEmpty(u) || string.Equals(i, u, StringComparison.OrdinalIgnoreCase))
            return i;
        return $"{i}, {u}";
    }

    private static Paragraph FinalReportPaperTitleParagraph(int sequenceInSection, Participant p)
    {
        var titlePart = FinalReportPaperTitleWithPeriod(p.PaperTitle);
        var org = FinalReportOrganizationParens(p);
        var orgPart = string.IsNullOrEmpty(org) ? "" : $" ({org})";
        var line = $"{sequenceInSection.ToString(CultureInfo.InvariantCulture)}. {titlePart}{orgPart}";

        var para = new Paragraph();
        para.AppendChild(FinalReportParagraphBase(JustificationValues.Both, FirstLineIndentHalfInch));
        var run = new Run();
        run.AppendChild(FinalReportRunTnr12(false));
        run.AppendChild(new Text(line));
        para.AppendChild(run);
        return para;
    }

    private static Paragraph FinalReportSpeakerParagraph(string fullName)
    {
        var para = new Paragraph();
        para.AppendChild(FinalReportParagraphBase(JustificationValues.Both, FirstLineIndentOneQuarterInch));
        var run = new Run();
        run.AppendChild(FinalReportRunTnr12(true));
        run.AppendChild(new Text($"Speaker: {fullName.Trim()}"));
        para.AppendChild(run);
        return para;
    }

    private static Paragraph? FinalReportSupervisorParagraph(Manager? manager)
    {
        if (manager == null || string.IsNullOrWhiteSpace(manager.FullName))
            return null;

        var name = manager.FullName.Trim();
        var deg = (manager.AcademicDegree ?? "").Trim();
        var text = string.IsNullOrEmpty(deg)
            ? $"Supervisor: {name}"
            : $"Supervisor: {deg} {name}";

        var para = new Paragraph();
        para.AppendChild(FinalReportParagraphBase(JustificationValues.Both, FirstLineIndentOneQuarterInch));
        var run = new Run();
        run.AppendChild(FinalReportRunTnr12(true));
        run.AppendChild(new Text(text));
        para.AppendChild(run);
        return para;
    }

    private static void WriteRegistrationWord(string path, Participant p)
    {
        using var wordDoc = WordprocessingDocument.Create(path, DocumentFormat.OpenXml.WordprocessingDocumentType.Document);
        var main = wordDoc.AddMainDocumentPart();
        main.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
        var body = main.Document.AppendChild(new Body());

        var table = new Table();
        foreach (var row in GetRegistrationRows(p))
        {
            var tr = new TableRow();
            tr.Append(CreateCell(row.Label), CreateCell(row.Value));
            table.Append(tr);
        }

        body.Append(table);
        main.Document.Save();
    }

    private static TableCell CreateCell(string text) =>
        new(
            new TableCellProperties(new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "2500" }),
            new Paragraph(new Run(new Text(text ?? ""))));

    private static IEnumerable<(string Label, string Value)> GetRegistrationRows(Participant p)
    {
        yield return ("Speaker full last name", p.FullName);
        yield return ("Educational institution", p.Institution);
        yield return ("Paper title", p.PaperTitle);
        yield return ("Phone", p.PhoneNumber);
        yield return ("Email", p.Email);
        yield return ("Section", p.SectionName);
        yield return ("Participation method", p.ParticipationMethod);
        yield return ("Paper file", p.DocumentPath ?? "");
        yield return ("Presentation file", p.PresentationPath ?? "");
        yield return ("Registration date (UTC)", p.RegistrationDate.ToString("u"));
        yield return ("Status", p.Status);
        if (p.Manager is null)
            yield return ("Manager", "(none)");
        else
        {
            yield return ("Manager full name", p.Manager.FullName);
            yield return ("Manager employment", p.Manager.PlaceOfEmployment);
            yield return ("Manager degree", p.Manager.AcademicDegree);
            yield return ("Manager position", p.Manager.Position);
        }
    }

    private static void WriteRegistrationExcel(string path, Participant p)
    {
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Registration");
        var rows = GetRegistrationRows(p).ToList();
        for (var i = 0; i < rows.Count; i++)
        {
            ws.Cell(i + 1, 1).Value = rows[i].Label;
            ws.Cell(i + 1, 2).Value = rows[i].Value;
        }

        ws.Columns().AdjustToContents();
        wb.SaveAs(path);
    }
}
