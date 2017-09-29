<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InstagramRegister.aspx.cs" Inherits="WebApiHash.Controllers.InstagramRegister" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="text-align: center">
    <form id="form1" runat="server" >
    <div >
        <asp:Label ID="LabelWelcome" runat="server" Text="WELCOME TO HASHTREND APP!" /><br /><br />
        <asp:Image ID="ImageInstagram" runat="server" ImageAlign="Middle" Height="120" Width="120" ImageUrl="https://www.seeklogo.net/wp-content/uploads/2016/05/instagram-icon-logo-vector-download.jpg" />
        
    </div>
        
        <p>
        
       <asp:Button ID="ButtonRegisterApp" runat="server" OnClick="ButtonRegisterApp_Click" Text="Login to our app by Instagram!" Height="100" Width="220px" />
        </p>
        
    </form>
</body>
</html>
