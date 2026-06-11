using System;

namespace EIskele.Application.Common.Settings;

public class MaintenanceModeSettingsDto
{
    public bool MaintenanceModeEnabled { get; set; } = false;
    public string MaintenanceMessage { get; set; } = "e-iskele kısa süreli bakım nedeniyle geçici olarak hizmet veremiyor.";
    public bool MaintenanceAffectsCustomerWeb { get; set; } = true;
    public bool MaintenanceAffectsCaptainHub { get; set; } = true;
    public bool MaintenanceAffectsAdminPanel { get; set; } = false;
    public bool MaintenanceAffectsPublicApi { get; set; } = true;
}
