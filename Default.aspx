<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="OIDCWithADFS.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <script src="./scripts/jquery-3.7.0.min.js"></script>
    <link href="./Content/bootstrap.min.css" rel="stylesheet" />
    <title>Login</title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="" style="border: 1px solid lightgray; position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); z-index: 1; width: 40%; border-radius: 5px; min-width: 300px;">
            <div class="mx-auto w-100 align-middle font-weight-bold p-3">
                <div class="mt-1 p-3 text-center fw-bold" style="background-color: #f2f2f2; font-size: 18px;">
                    Login
                </div>
            </div>
            <br />
            <table class="table table-borderless mx-auto text-center " style="color: #fff; font-size: 14px; width: 95%;">
                <tr>
                    <td class="align-middle">
                        <asp:Button ID="btn" runat="server" Text="로그인" CssClass="btn btn-lg font-weight-bold w-100 btn-dark" OnClick="btnLogin" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>