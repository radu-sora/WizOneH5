<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DateGenerale.ascx.cs" Inherits="WizOne.Recrutare.DateGenerale" %>

<%@ Register assembly="DevExpress.Web.v18.1, Version=18.1.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.Web" tagprefix="dx" %>

    <style type="text/css">
        .buttonAlign {
            text-align: right;
        }
        @media(max-width:600px) {
            .buttonAlign {
                text-align: center;
            }
        }
    </style>

    <dx:ASPxFormLayout ID="frmGen" runat="server" AlignItemCaptionsInAllGroups="True" UseDefaultPaddings="false">
        <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="600" />
        <Items>
            <dx:LayoutGroup GroupBoxDecoration="None" UseDefaultPaddings="false">
                <Items>
                    <dx:LayoutItem Caption="Canal recrutare" Width="33%">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                <dx:ASPxComboBox ID="cmbCanal" runat="server" ValueField="Id" DropDownWidth="200" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                </Items>
            </dx:LayoutGroup>
            <dx:EmptyLayoutItem />
            <dx:LayoutGroup Caption="Date de identificare" ColCount="2">
                <Items>
                    <dx:LayoutItem Caption="Nume" FieldName="Nume">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                <dx:ASPxTextBox ID="txtNume" runat="server" Width="100%" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="Prenume" FieldName="Prenume">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                <dx:ASPxTextBox ID="txtPrenume" runat="server" Width="100%" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="Data nastere" FieldName="DataNastere">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                <dx:ASPxDateEdit ID="txtDataNastere" runat="server" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="CNP" FieldName="CNP">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                <dx:ASPxTextBox ID="txtCNP" runat="server" MaxLength="13">
                                    <MaskSettings IncludeLiterals="None" />
                                </dx:ASPxTextBox>
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                </Items>
            </dx:LayoutGroup>
            <dx:LayoutGroup Caption="Adresa" ColCount="2">
                <Items>
                    <dx:LayoutItem Caption="Judet" FieldName="IdJudet">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                <dx:ASPxComboBox ID="cmbJud" runat="server" ValueField="Id" DropDownWidth="200" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="Localitate" FieldName="Localitate">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                <dx:ASPxTextBox ID="txtLoc" runat="server" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="Adresa" Width="100%" FieldName="AdresaCompleta">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                <dx:ASPxMemo ID="txtAdr" runat="server" Height="71px" Width="100%" Rows="8" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                </Items>
            </dx:LayoutGroup>
            <dx:LayoutItem ShowCaption="False" CssClass="buttonAlign" Width="100%">
                <LayoutItemNestedControlCollection>
                    <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                        <dx:ASPxButton ID="btnUpdate" runat="server" Text="Update Record" OnClick="btnUpdate_Click" Width="150" />
                    </dx:LayoutItemNestedControlContainer>
                </LayoutItemNestedControlCollection>
            </dx:LayoutItem>
        </Items>
    </dx:ASPxFormLayout>
