var tra_OperatieNepermisa = "Operatie nepermisa";
var tra_NevoieMotiv = "Pentru a putea respinge este nevoie de un motiv";
var trad_NuPutetiAnula = "Nu puteti anula o cerere deja anulata sau respinsa";


function trad_string(idLimba, expresie)
{
    var rez = expresie;

    switch(expresie)
    {
        case "Operatie nepermisa":
                if (idLimba == "EN") rez = "Unauthorized operation";
            break;
        case "Pentru a putea respinge este nevoie de un motiv":
                if (idLimba == "EN") rez = "To be able to reject a reason is needed";
            break;
        case "Nu puteti anula o cerere deja anulata sau respinsa":
                if (idLimba == "EN") rez = "You can not cancel an already canceled or rejected request";
            break;
        case "Cererea va fi anulata !":
            if (idLimba == "EN") rez = "Request will be canceled !";
            break;
        case "Sunteti sigur/a ?":
            if (idLimba == "EN") rez = "Are you sure ?";
            break;
        case "Da, anuleaza!":
            if (idLimba == "EN") rez = "Yes, cancel it !";
            break;
        case "Renunta":
            if (idLimba == "EN") rez = "Dissmis";
            break;
        case "Atentie !":
            if (idLimba == "EN") rez = "Attention !";
            break;
        case "Se pot diviza numai cererile in starea aprobat":
            if (idLimba == "EN") rez = "Only applications in the approved state can be divided";
            break;
        case "Intervalul ales trebuie sa fie de minim 2 zile":
            if (idLimba == "EN") rez = "The chosen interval must be at least 2 days";
            break;
        case "Lipseste data cu care se divide cererea":
            if (idLimba == "EN") rez = "Missing the date the application is split";
            break;
        case "Data nu este in intervalul din cerere":
            if (idLimba == "EN") rez = "The date is not in the range of the request";
            break;
        case "Vreti sa continuati procesul de respingere ?":
            if (idLimba == "EN") rez = "Do you want to continue the rejection process?";
            break;
        case "Da, continua!":
            if (idLimba == "EN") rez = "Yes, continue!";
            break;
        case "Nu exista linii selectate":
            if (idLimba == "EN") rez = "There are no selected lines";
            break;
        case "Vreti sa continuati procesul de aprobare ?":
            if (idLimba == "EN") rez = "Do you want to continue the approval process ?";
            break;
        case "Operatie":
            if (idLimba == "EN") rez = "Unauthorized";
            break;
    }

    return rez;
}



