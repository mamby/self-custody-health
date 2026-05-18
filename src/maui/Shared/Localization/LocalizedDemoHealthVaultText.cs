using SelfCustodyHealth.Domain;

namespace SelfCustodyHealth.Shared.Localization;

internal static class LocalizedDemoHealthVaultText
{
	public static DemoHealthVaultText Create() =>
		new()
		{
			EmergencyContactName = AppText.Get("DemoEmergencyContactName"),
			EmergencyContactRelationship = AppText.Get("DemoEmergencyContactRelationship"),
			EmergencyContactNotes = AppText.Get("DemoEmergencyContactNotes"),
			BloodType = "O+",
			Allergy = AppText.Get("DemoPenicillin"),
			ChronicCondition = AppText.Get("DemoMildAsthma"),
			Surgery = AppText.Get("DemoAppendectomy"),
			AnnualBloodPanelTitle = AppText.Get("DemoAnnualBloodPanelTitle"),
			AnnualBloodPanelSource = AppText.Get("DemoAnnualBloodPanelSource"),
			AnnualBloodPanelNotes = AppText.Get("DemoAnnualBloodPanelNotes"),
			AsthmaInhalerPrescriptionTitle = AppText.Get("DemoAsthmaInhalerPrescriptionTitle"),
			PrimaryCareSource = AppText.Get("DemoPrimaryCare"),
			AsthmaInhalerPrescriptionNotes = AppText.Get("DemoAsthmaInhalerPrescriptionNotes"),
			FluVaccinationReceiptTitle = AppText.Get("DemoFluVaccinationReceiptTitle"),
			CommunityPharmacySource = AppText.Get("DemoCommunityPharmacy"),
			FluVaccinationReceiptNotes = AppText.Get("DemoFluVaccinationReceiptNotes"),
			MaintenanceInhalerName = AppText.Get("DemoMaintenanceInhalerName"),
			MaintenanceInhalerDose = AppText.Get("DemoMaintenanceInhalerDose"),
			MaintenanceInhalerInstructions = AppText.Get("DemoMaintenanceInhalerInstructions"),
			PrimaryCareCheckInTitle = AppText.Get("DemoPrimaryCareCheckInTitle"),
			PrimaryCareCheckInClinician = AppText.Get("DemoPrimaryCareCheckInClinician"),
			DowntownClinicLocation = AppText.Get("DemoDowntownClinic"),
			BringRecentLabResultNotes = AppText.Get("DemoBringRecentLabResult"),
			InfluenzaName = AppText.Get("DemoInfluenza"),
			TakeMaintenanceInhalerTitle = AppText.Get("DemoTakeMaintenanceInhaler"),
			ProfileDisplayName = AppText.Get("DemoProfileDisplayName")
		};
}
