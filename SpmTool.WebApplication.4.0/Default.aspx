<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SpmTool.WebApplication.Convert" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title></title>
        <style type="text/css">
            .auto-style1
            {
                width: 155px;
                height: 165px;
            }

            body
            {
              background: #ffffff;
              font-family: Arial, Helvetica, sans-serif;
              font-size: 12px;
            }

            h3
            {
                font-weight: normal;
                color: #e76816;
                font-size: 1.75em;
                font-family: 'TeXGyreHerosRegular', Arial, Helvetica, sans-serif;
                text-transform: uppercase;
                letter-spacing: 0.035em;
                line-height: 1.15em;
            }
        </style>
    </head>
    <body>
        <form id="form1" runat="server">

            <h3>Spektrum DX7s/DX8 to DX9/DX18 conversion tool</h3>

            <p>
                <img alt="DX7s" class="auto-style1" src="dx7s-small.png" />
                <img alt="DX8" class="auto-style1" src="dx8-small.png" />
                <img alt="DX9" class="auto-style1" src="dx9-small.png" />
                <img alt="DX18" class="auto-style1" src="dx18-small.png" /></p>
            <p>
            Please subscribe to this <a href="http://www.rcgroups.com/forums/showthread.php?t=2083054">thread</a> for more information/updates.
            </p>

            <p>
                Contact email address:<br />
                <asp:TextBox ID="contactTextBox" runat="server"></asp:TextBox>
            </p>

            <p>
                Select a 
                ZIP file that contains multiple model files or an individual SPM file:<br />
                <asp:FileUpload ID="fileUpload" runat="server" />
            </p>

            <p>
                Convert for which radio:<br />
                <asp:DropDownList ID="targetGenerator"  runat="server">
                <asp:ListItem>DX9</asp:ListItem>
                <asp:ListItem>DX18</asp:ListItem>
                <asp:ListItem>DX8</asp:ListItem>
                </asp:DropDownList>
            </p>

            <p>
                <asp:CheckBox ID="removeIndexCheckBox" runat="server" Text="Remove slot index from model name." />
            </p>

            <p>
                    Pressing 'Convert' will upload and then return your converted file. Please wait a few seconds while this happens.
                    <br />
                    <asp:Button ID="uploadButton" runat="server" EnableViewState="False" OnClick="uploadButton_Click" Text="Convert" />
            </p>

            <asp:Label ID="statusLabel" runat="server" ForeColor="Red"></asp:Label>
        </form>
    </body>
</html>
