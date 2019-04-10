<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DateGenerale.ascx.cs" Inherits="WizOne.Recrutare.DateGenerale" %>



<div style="max-width:1300px;">
    <dx:ASPxFormLayout ID="frmGen" runat="server" AlignItemCaptionsInAllGroups="false" UseDefaultPaddings="false" ColumnCount="2">
        <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="600" />
        <Items>
            <dx:LayoutGroup GroupBoxDecoration="None" UseDefaultPaddings="false" ColumnSpan="2">
                <Items>
                    <dx:LayoutItem Caption="Canal recrutare" FieldName="CanalRecrutare">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                <dx:ASPxComboBox ID="cmbCanal" runat="server" ValueField="Id" TextField="Denumire" ValueType="System.Int32" AutoPostBack="false" Width="150" />
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
                                <dx:ASPxTextBox ID="txtNume" runat="server" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="Prenume" FieldName="Prenume">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                <dx:ASPxTextBox ID="txtPrenume" runat="server" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="Data nastere" FieldName="DataNastere">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                <dx:ASPxDateEdit ID="txtDataNastere" runat="server" DisplayFormatString="dd/MM/yyyy" EditFormatString="dd/MM/yyyy" EditFormat="Custom" />
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

            <dx:LayoutGroup Caption="Date de contact" ColCount="2">
                <Items>
                    <dx:LayoutItem Caption="Mail" FieldName="Mail">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                <dx:ASPxTextBox ID="txtMail" runat="server"  />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="Telefon" FieldName="Telefon">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                <dx:ASPxTextBox ID="txtTel" runat="server"  />
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

            <dx:LayoutGroup Caption="Social Media" ColCount="1">
                <Items>
                    <dx:LayoutItem Caption="LinkedIn" FieldName="Linkedin">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                <dx:ASPxTextBox ID="txtLinkedin" runat="server" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="Facebook" FieldName="Facebook">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                <dx:ASPxTextBox ID="txtFacebook" runat="server" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="Twitter" FieldName="Twitter">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                <dx:ASPxTextBox ID="txtTwitter" runat="server" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="Instagram" FieldName="Instagram">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                <dx:ASPxTextBox ID="txtInstagram" runat="server" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                </Items>
            </dx:LayoutGroup>

            <dx:LayoutGroup Caption="Diverse" ColCount="2">
                <Items>
                    <dx:LayoutItem Caption="Permis de conducere" FieldName="Permis">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                <dx:ASPxCheckBox ID="chkPermis" runat="server" Text="" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="Dispus pentru deplasari" FieldName="Deplasari">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                <dx:ASPxCheckBox ID="chkDeplasare" runat="server" Text="" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                    <dx:LayoutItem Caption="Dispus pentru relocare" FieldName="Relocare">
                        <LayoutItemNestedControlCollection>
                            <dx:LayoutItemNestedControlContainer runat="server" SupportsDisabledAttribute="True">
                                <dx:ASPxCheckBox ID="chkRelocare" runat="server" Text="" />
                            </dx:LayoutItemNestedControlContainer>
                        </LayoutItemNestedControlCollection>
                    </dx:LayoutItem>
                </Items>
            </dx:LayoutGroup>
        </Items>
    </dx:ASPxFormLayout>
</div>