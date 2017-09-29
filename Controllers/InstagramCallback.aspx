<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InstagramCallback.aspx.cs" Inherits="WebApiHash.Controllers.InstagramCallback" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server" style="text-align:center">
    <div>
             
           <p style="text-align: left">
            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="http://hashtrend.hostingasp.pl/Controllers/instagramRegister.aspx">Home</asp:HyperLink>&nbsp;&nbsp;

    <asp:Image ID="ImageInstagram" runat="server" ImageAlign="Middle" Height="120" Width="120" ImageUrl="https://www.seeklogo.net/wp-content/uploads/2016/05/instagram-icon-logo-vector-download.jpg" /><br /><br />
        <asp:Label ID="Label" runat="server" Text="Let's try to find the media, that you are interested in!"></asp:Label>
        <br />
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>

    </div>
        <p>
        <asp:Button ID="btn" runat="server" Text="Search for it!" OnClick="btn_Click" />

        </p>
        <asp:Image ID="Image1" runat="server" />
        <asp:Image ID="Image2" runat="server" />
        <asp:Image ID="Image3" runat="server" />
        <asp:Image ID="Image4" runat="server" />
        <asp:Image ID="Image5" runat="server" />
        <p>
            <asp:Image ID="Image6" runat="server" />
            <asp:Image ID="Image7" runat="server" />
            <asp:Image ID="Image8" runat="server" />
            <asp:Image ID="Image9" runat="server" />
            <asp:Image ID="Image10" runat="server" />
        </p>
        <p>
            <asp:Image ID="Image12" runat="server" />
            <asp:Image ID="Image11" runat="server" />
            <asp:Image ID="Image13" runat="server" />
            <asp:Image ID="Image14" runat="server" />
            <asp:Image ID="Image15" runat="server" />
        </p>
        <p>
            <asp:Image ID="Image16" runat="server" />
            <asp:Image ID="Image17" runat="server" />
            <asp:Image ID="Image18" runat="server" />
            <asp:Image ID="Image19" runat="server" />
            <asp:Image ID="Image20" runat="server" />
        </p>
        <p>
            &nbsp;</p>


    </form>
</body>
</html>
