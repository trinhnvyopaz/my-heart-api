using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHeart.Data.Migrations
{
    public partial class PdfTemplateSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
GO
SET IDENTITY_INSERT [dbo].[PdfTemplate] ON 
GO
INSERT [dbo].[PdfTemplate] ([Id], [LastUpdateDate], [LastUpdateUser], [Code], [Template]) VALUES (1, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'UNDEFINED', N'HealthStatus', N'<!DOCTYPE html
    PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""cs"" lang=""cs"">

<head>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
    <title>PDF export Moje srdce Vzor Zdravotní informace</title>
    <style type=""text/css"">
        * {
            margin: 0;
            padding: 0;
            text-indent: 0;
        }

        .s1 {
            color: black;
            font-family: Arial, sans-serif;
            font-style: normal;
            font-weight: normal;
            text-decoration: none;
            font-size: 12pt;
        }

        .a {
            color: #1154CC;
            font-family: Arial, sans-serif;
            font-style: normal;
            font-weight: normal;
            text-decoration: underline;
            font-size: 12pt;
        }

        .s2 {
            color: #1154CC;
            font-family: Arial, sans-serif;
            font-style: normal;
            font-weight: normal;
            text-decoration: none;
            font-size: 12pt;
        }

        h1 {
            color: black;
            font-family: Arial, sans-serif;
            font-style: normal;
            font-weight: bold;
            text-decoration: none;
            font-size: 20pt;
        }

        p {
            color: black;
            font-family: Arial, sans-serif;
            font-style: normal;
            font-weight: normal;
            text-decoration: none;
            font-size: 12pt;
            margin: 0pt;
        }

        h2 {
            color: black;
            font-family: Arial, sans-serif;
            font-style: normal;
            font-weight: bold;
            text-decoration: none;
            font-size: 12pt;
        }

        .s3 {
            color: black;
            font-family: ""Times New Roman"", serif;
            font-style: normal;
            font-weight: normal;
            text-decoration: none;
            font-size: 12pt;
        }

        .s4 {
            color: black;
            font-family: Arial, sans-serif;
            font-style: italic;
            font-weight: bold;
            text-decoration: none;
            font-size: 12pt;
        }
		.value-row > *{
            padding-top: 1pt;
            padding-left: 41pt;
            text-indent: 0pt;
            text-align: left;
        }
        .personal-info-row > *{
            display: inline-block;
            text-align: left;
            padding-left: 5pt;
        }
        .personal-info-row-title{
            width: 150px;
        }
    </style>
</head>

<body>
    <p style=""text-align: center;"">
        <img  width=""160"" height=""55""
            src=""data:image/png;base64,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"" />
    </p>
    <p class=""s1"" style=""padding-top: 5pt;text-align: center;"">Moje srdce s.r.o.</p>
    <p class=""s1"" style=""text-align: center;"">Oblouková 848/25, Praha 10, 101 00</p>
    <p class=""s1"" style=""text-align: center;"">IČO: 10688099</p>
    <p class=""s2"" style=""text-align: center;""><a href=""http://www.mojesrdce.cz/""class=""a"" target=""_blank"">www.mojesrdce.cz</a> 
    <p class=""s1"" style=""text-align: center;""><span style="" color: #000;"">tel. 601377723</span></p></p>

    <p style=""text-indent: 0pt;text-align: left;""><br /></p>
    <h1 style=""padding-top: 7pt;text-align: center;"">{{Title}}</h1>
    <p style=""text-indent: 0pt;text-align: left;""><br /></p>
 
    <div class=""personal-info-row"">
        <div class=""personal-info-row-title s1"">Příjmení a jméno:</div>   
        <p class=""s1"" style=""text-align: center;""><b>{{Name}}</b></div>
    </div>
 
    <div class=""personal-info-row"">
        <div class=""personal-info-row-title s1"">Rodné číslo:</div>   
        <p class=""s1""><b>{{PIN}}</b></p>
    </div>
    <div class=""personal-info-row"">
        <div class=""personal-info-row-title s1"">Datum narození:</div>   
        <p class=""s1""><b>{{InsuranceNumber}}</b></dpiv>
    </div>
    <div class=""personal-info-row"">
        <div class=""personal-info-row-title s1"">Kontaktní adresa:</div>   
        <p class=""s1"">{{ContactAddress}}</p>
    </div>
    <div class=""personal-info-row"">
        <div class=""personal-info-row-title s1"">Telefonní kontakt:</div>   
        <p class=""s1"">{{PhoneContact}}</p>
    </div>

    <p style=""text-indent: 0pt;text-align: left;""><br /></p>
    <p class=""s4"" style=""padding-left: 5pt;text-indent: 0pt;text-align: left;"">Export ze systému mojesrdce.cz ze dne
        {{CurrentDate}}</p>
    <p style=""text-indent: 0pt;text-align: left;""><br /></p>
    <h2 style=""padding-left: 5pt;text-indent: 0pt;text-align: left;"">Onemocnění:</h2>
        {{#each Diseases}}
            <p style=""padding-top: 1pt;padding-left: 41pt;text-indent: 0pt;line-height: 114%;text-align: left;"">{{Disease.Name}} {{formatDate StartDateString ""yyyy""}} {{Note}}</p>       
        {{/each}}
    <h2 style=""padding-top: 1pt;padding-left: 5pt;text-indent: 0pt;text-align: left;"">Provedené výkony:</h2>
        {{#each NonpharmaticTherapies}}
            <p style=""padding-top: 1pt;padding-left: 41pt;text-indent: 0pt;line-height: 114%;text-align: left;"">{{Disease.Name}} {{formatDate StartDateString ""yyyy""}} {{Note}}</p>       
        {{/each}}        
    <h2 style=""padding-left: 5pt;text-indent: 0pt;text-align: left;"">Abusus:</h2>
        {{#each abusus}}
            <p style=""padding-top: 1pt;padding-left: 41pt;text-indent: 0pt;line-height: 114%;text-align: left;"">{{this}}</p>       
        {{/each}}    
    <h2 style=""padding-top: 1pt;padding-left: 5pt;text-indent: 0pt;text-align: left;"">Užívané léky:</h2>
        {{#each pharmacies}}
            <p style=""padding-top: 1pt;padding-left: 41pt;text-indent: 0pt;line-height: 114%;text-align: left;"">{{IsPharmacy_Name}} {{IsPharmacy_Dose}} {{IsPharmacy_MorningDose}}-{{IsPharmacy_AfternoonDose}}-{{IsPharmacy_EveningDose}} {{IsPharmacy_Note}}</p>       
        {{/each}}    
    <h2 style=""padding-left: 5pt;text-indent: 0pt;text-align: left;"">Zdravotní stav:</h2>
    <p style=""padding-top: 1pt;padding-left: 41pt;text-indent: 0pt;text-align: left;"">Výška: {{height}}cm, Váha: {{weight}}kg, BMI:
        {{bmi}}</p>
    <p style=""padding-top: 1pt;padding-left: 41pt;text-indent: 0pt;line-height: 114%;text-align: left;"">Tlak krve:
        {{bloodPressure}} mmHg, tepová frekvence: {{heartRate}}/min. LDL cholesterol {{ldl}} mmol/l</p>
    <h2 style=""padding-left: 5pt;text-indent: 0pt;text-align: left;"">Doporučení:</h2>
        {{#each recommendations}}
              <div class=""value-row"">{{{this}}}</div>
        {{/each}}   
    <p style=""text-indent: 0pt;text-align: left;""><br /></p>
    <p style=""padding-left: 5pt;text-indent: 0pt;line-height: 114%;text-align: left;"">Automaticky generováno systémem
        mojesrdce.cz dne {{currentDate}} {{currentTime}} Kontakt mojesrdce.cz, 601377723</p>
    <p style=""text-indent: 0pt;text-align: left;""><br /></p>
</body>

</html>')
GO
INSERT [dbo].[PdfTemplate] ([Id], [LastUpdateDate], [LastUpdateUser], [Code], [Template]) VALUES (2, CAST(N'2023-01-24T17:46:48.7533333' AS DateTime2), N'UNDEFINED', N'Pharmacies', N'<!DOCTYPE html>

<html xmlns=""http://www.w3.org/1999/xhtml"">

<head>
    <meta charset=""utf-8"" />
    <meta name=""generator"" content=""pdf2htmlEX"" />
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge,chrome=1"" />
    <style type=""text/css"">
        .row > *{
            display: inline-block;
        }
        td {
            border-style: solid;
            border-width: 2px;
            border-spacing: 0;
        }
        .border-left-none{
            border-left: 0px;
        }
        .border-top-none{
            border-top: 0px;
        }
        table{
            width: 500px;
        }
        table td {
            padding: 10px;
            width: 120px;
        }
        *{
            font-family: Arial, sans-serif;
            font-size: 12pt;
        }
        .footer-text{
            font-size: 10pt;
        }
        .column{
            width: 464px;
        }
        .small-column{
            width: 36px;
        }
    </style>

<body>
    <div class=""row"">
        <div class=""column"">
            <div class=""row"">
                <span><b>{{Name}}, </b></span>
                <span>rodné číslo <b>{{PIN}}</b>, </span>
                <span>poj. <b>{{InsuranceNumber}}</b></span>
                
            </div>
        
            <div class=""row"">
                <span><b>Užívaná léčba</b> dne {{CurrentDate}}</span>
            </div>
        </div>
        <div class=""small-column"">
            <img width=""36"" height=""36""
            src=""data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACQAAAAkCAYAAADhAJiYAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAA7EAAAOxAGVKw4bAAADiUlEQVRYhcVXv0srWRT+zmRjfiASK4tNn+qBYCPbabGNIKmsxOpt4Z+Qv8PqEVkQ9DUWdgui1osWsmhQXohECbwuoCJhEp3zbeHMOL/uZJSnHrjc+e45596Tb849ZyIYL8UMNq8R+y1O/wGgOzTw/CvxzyyBVAJOwY3eE38xBfP7OzGRBf/pBSGBgDiWv/cVAQDLBW9KtF8sCrwwpO4sCDP10diyAJQRfnXyibhmAfgL8fxJxN1uFyShqlRVTE1NxewbjYavJ4lms2ncLwF/B4C/Meaa1ut1VVVtNBq+Pp/PK0k6juPbk+T9/X3Iv9VqUVWzloEfXkBGw8nJSSVp1J+dnamqEgDL5XLiQaVSie4e4wLzA9KI0sckKSJGPQBeXV3paDQy6gHo4eGhnpycGPUmhmIjwE7qUFUeHR2l2mTY68dvSJG5uTk8Pj6mmfhiWRZIolAoYDgcZvIxiZGh4XCYmSEAzOVyqfYkubS0lMpQMCCNziS9G5KoT5r7/b5ubGzE1ldWVvTh4WHcfuk5RJK7u7v0blHWkcSStzaG8cSAFADX19d5enpKADoYDNjpdEL6qH0Qn5+fc2try8ckuby87CW/WpZl8jczpKqcmJgI4Xq9/mqWBoMBu92uv762tsabm5vXMxQphv4h+Xw+03eOqnJnZyeYMyHGTAx5157u7DU61+cFA0AulxPHcUREYvZRXKvV0G63RURiem/zBH//eyjUgff29mRzczPWkZ9/LKCqYzt4u9026judjqyurhr1sRxKuwmqylarRdu2U/NnZmaGqspSqRTTi4jpjFgv4zMBoe7s953j42M2Gg31bLa3t0N6N2BtNpsKQBcWFnh7e5vYtyL1KNbLfAcRMQYU6dhKktVq1ce9Xo+2bccCNAVULpdjAXk55L1HFAoFCSR06iwi6PV6Mj8/j4ODA6lWqygWiyE7N6lj/nd3d1IqlWLrVsQYtm3DsqxgwgGAXF9fo9VqIWovItjf3xfHcfBy9ou+2WxiNBrF9qtUKuj3+zF7ICGpi8Uiva/Bi4sLkuTT01Pmohgdtm2TJC8vL+k4DkmyVqsZk/oPGApcpVLh4uIi3bqTqSCm4dnZWU5PT6fZf/Oo8guT+xz9A/lRuJgU0GeKeEn99VPDeJbvQJg6xee9KsK98dHr6AX10XnkPwcLo4d/JgT6XniQoDPKPxj/x+6t+F/Tof8D+PUa8Yic3kMAAAAASUVORK5CYIIA"" />
        </div>
    </div>

    <br>
    <table cellspacing=""0"">
        <tr>
            <td class=""border-bottom-none"">Název</td>
            <td class=""border-left-none"">Síla</td>
            <td class=""border-left-none"">Dávkování</td>
            <td class=""border-left-none"">Poznámka</td>
        </tr>
            {{#each Pharmacies}}
                <tr>
                    <td class=""border-top-none"">{{IsPharmacy_Name}}</td>
                    <td class=""border-top-none border-left-none"">{{IsPharmacy_Dose}}</td>
                    <td class=""border-top-none row border-left-none"">{{IsPharmacy_MorningDose}}-{{IsPharmacy_AfternoonDose}}-{{IsPharmacy_EveningDose}}</td>
                    <td class=""border-top-none border-left-none"">{{IsPharmacy_Note}}</td>
                </tr>
            {{/each}}
    </table>
    <br>    
    <div>
        <div class=""footer-text"">
            Automaticky generováno systémem mojesrdce.cz dne {{CurrentDate}}<br> kontakt: mojesrdce.cz, tel. 601377723
        </span>
    </div>

</body>

</html>')
GO
INSERT [dbo].[PdfTemplate] ([Id], [LastUpdateDate], [LastUpdateUser], [Code], [Template]) VALUES (3, CAST(N'2023-01-25T15:22:34.7433333' AS DateTime2), N'KL', N'Chat', N'<!DOCTYPE html
    PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"" xml:lang=""cs"" lang=""cs"">

<head>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
    <title>PDF export Moje srdce Vzor Zdravotní informace</title>
    <style type=""text/css"">
        * {
            margin: 0;
            padding: 0;
            text-indent: 0;
        }

        .s1 {
            color: black;
            font-family: Arial, sans-serif;
            font-style: normal;
            font-weight: normal;
            text-decoration: none;
            font-size: 12pt;
        }

        .a {
            color: #1154CC;
            font-family: Arial, sans-serif;
            font-style: normal;
            font-weight: normal;
            text-decoration: underline;
            font-size: 12pt;
        }

        .s2 {
            color: #1154CC;
            font-family: Arial, sans-serif;
            font-style: normal;
            font-weight: normal;
            text-decoration: none;
            font-size: 12pt;
        }

        h1 {
            color: black;
            font-family: Arial, sans-serif;
            font-style: normal;
            font-weight: bold;
            text-decoration: none;
            font-size: 20pt;
        }

        p {
            color: black;
            font-family: Arial, sans-serif;
            font-style: normal;
            font-weight: normal;
            text-decoration: none;
            font-size: 12pt;
            margin: 0pt;
        }

        h2 {
            color: black;
            font-family: Arial, sans-serif;
            font-style: normal;
            font-weight: bold;
            text-decoration: none;
            font-size: 12pt;
        }

        .s3 {
            color: black;
            font-family: ""Times New Roman"", serif;
            font-style: normal;
            font-weight: normal;
            text-decoration: none;
            font-size: 12pt;
        }

        .s4 {
            color: black;
            font-family: Arial, sans-serif;
            font-style: italic;
            font-weight: bold;
            text-decoration: none;
            font-size: 12pt;
        }
		.value-row > *{
            padding-top: 1pt;
            padding-left: 41pt;
            text-indent: 0pt;
            text-align: left;
        }
        .personal-info-row > *{
            display: inline-block;
            text-align: left;
            padding-left: 5pt;
        }
        .personal-info-row-title{
            width: 150px;
        }
    </style>
</head>

<body>
    <p style=""text-align: center;"">
        <img  width=""160"" height=""55""
            src=""data:image/png;base64,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"" />
    </p>
    <p class=""s1"" style=""padding-top: 5pt;text-align: center;"">Moje srdce s.r.o.</p>
    <p class=""s1"" style=""text-align: center;"">Oblouková 848/25, Praha 10, 101 00</p>
    <p class=""s1"" style=""text-align: center;"">IČO: 10688099</p>
    <p class=""s2"" style=""text-align: center;""><a href=""http://www.mojesrdce.cz/""class=""a"" target=""_blank"">www.mojesrdce.cz</a> 
    <p class=""s1"" style=""text-align: center;""><span style="" color: #000;"">tel. 601377723</span></p></p>

    <p style=""text-indent: 0pt;text-align: left;""><br /></p>
    <h1 style=""padding-top: 7pt;text-align: center;"">{{Title}}</h1>
    <p style=""text-indent: 0pt;text-align: left;""><br /></p>
 
    <div class=""personal-info-row"">
        <div class=""personal-info-row-title s1"">Příjmení a jméno:</div>   
        <p class=""s1"" style=""text-align: center;""><b>{{Name}}</b></div>
    </div>
 
    <div class=""personal-info-row"">
        <div class=""personal-info-row-title s1"">Rodné číslo:</div>   
        <p class=""s1""><b>{{PIN}}</b></p>
    </div>
    <div class=""personal-info-row"">
        <div class=""personal-info-row-title s1"">Datum narození:</div>   
        <p class=""s1""><b>{{InsuranceNumber}}</b></dpiv>
    </div>
    <div class=""personal-info-row"">
        <div class=""personal-info-row-title s1"">Kontaktní adresa:</div>   
        <p class=""s1"">{{ContactAddress}}</p>
    </div>
    <div class=""personal-info-row"">
        <div class=""personal-info-row-title s1"">Telefonní kontakt:</div>   
        <p class=""s1"">{{PhoneContact}}</p>
    </div>

    <p style=""text-indent: 0pt;text-align: left;""><br /></p>
    <p class=""s4"" style=""padding-left: 5pt;text-indent: 0pt;text-align: left;"">Export ze systému mojesrdce.cz ze dne
        {{CurrentDate}}</p>
    <p style=""text-indent: 0pt;text-align: left;""><br /></p>
    
    <div>
        <span style=""display: block;""><b>Předmět</b></span>
        <span style=""text-align: center;display: block;""><b>{{Subject}}</b></span><br>
    </div>

    <div>
        <span style=""display: block;""><b>Datum založení</b></span>
        <span style=""text-align: center;display: block;""><b>{{CreatedAt}}</b></span><br>
    </div>

    <div>
        {{#each Messages}}
            <b>{{Type}}: </b> <span>{{Text}}</span><br><br>
        {{/each }}
    </div>

    <p style=""text-indent: 0pt;text-align: left;""><br /></p>
    <p style=""padding-left: 5pt;text-indent: 0pt;line-height: 114%;text-align: left;"">Automaticky generováno systémem
        mojesrdce.cz dne {{currentDate}} {{currentTime}} Kontakt mojesrdce.cz, 601377723</p>
    <p style=""text-indent: 0pt;text-align: left;""><br /></p>
</body>

</html>')
GO
SET IDENTITY_INSERT [dbo].[PdfTemplate] OFF
GO
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from PdfTemplate where id in (1,2,3)");
        }
    }
}
