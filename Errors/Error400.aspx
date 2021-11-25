<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error400.aspx.cs" Inherits="protean.Errors.Error400" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>Bad Request | Grand Rapids Chair Co</title>
    <meta name="robots" content="nofollow" />
    <meta name="robots" content="noindex" />
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <link href="../Content/vendor/fontawesome-free/css/all.min.css" rel="stylesheet" type="text/css">
    <link href="https://fonts.googleapis.com/css?family=Nunito:200,200i,300,300i,400,400i,600,600i,700,700i,800,800i,900,900i" rel="stylesheet">

    <link href="../Content/css/sb-admin-2.css" rel="stylesheet" />
</head>

<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1><i class="fa fa-sad-tear"></i></h1>
            <h2>Oops, something went wrong.</h2>

            <p class="lead">
                Details of this error have been sent to our information technology wizards.  If this is an urgent matter,
				please give us a call.
            </p>

            <p class="lead">
                <a href="/" title="portal">Click here</a> to get back to the portal.
            </p>
        </div>
    </form>
</body>
</html>
