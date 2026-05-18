namespace PersonalHealthVault.Domain;

internal sealed record DemoHealthVaultText
{
	public required string EmergencyContactName { get; init; }
	public required string EmergencyContactRelationship { get; init; }
	public required string EmergencyContactNotes { get; init; }
	public required string BloodType { get; init; }
	public required string Allergy { get; init; }
	public required string ChronicCondition { get; init; }
	public required string Surgery { get; init; }
	public required string AnnualBloodPanelTitle { get; init; }
	public required string AnnualBloodPanelSource { get; init; }
	public required string AnnualBloodPanelNotes { get; init; }
	public required string AsthmaInhalerPrescriptionTitle { get; init; }
	public required string PrimaryCareSource { get; init; }
	public required string AsthmaInhalerPrescriptionNotes { get; init; }
	public required string FluVaccinationReceiptTitle { get; init; }
	public required string CommunityPharmacySource { get; init; }
	public required string FluVaccinationReceiptNotes { get; init; }
	public required string MaintenanceInhalerName { get; init; }
	public required string MaintenanceInhalerDose { get; init; }
	public required string MaintenanceInhalerInstructions { get; init; }
	public required string PrimaryCareCheckInTitle { get; init; }
	public required string PrimaryCareCheckInClinician { get; init; }
	public required string DowntownClinicLocation { get; init; }
	public required string BringRecentLabResultNotes { get; init; }
	public required string InfluenzaName { get; init; }
	public required string TakeMaintenanceInhalerTitle { get; init; }
	public required string ProfileDisplayName { get; init; }

	public static DemoHealthVaultText English { get; } = new()
	{
		EmergencyContactName = "Alex Morgan",
		EmergencyContactRelationship = "Partner",
		EmergencyContactNotes = "Primary emergency contact",
		BloodType = "O+",
		Allergy = "Penicillin",
		ChronicCondition = "Mild asthma",
		Surgery = "Appendectomy, 2018",
		AnnualBloodPanelTitle = "Annual blood panel",
		AnnualBloodPanelSource = "Northside Lab",
		AnnualBloodPanelNotes = "Demo document. Replace with your own encrypted local records.",
		AsthmaInhalerPrescriptionTitle = "Asthma inhaler prescription",
		PrimaryCareSource = "Primary care",
		AsthmaInhalerPrescriptionNotes = "Demo prescription record.",
		FluVaccinationReceiptTitle = "Flu vaccination receipt",
		CommunityPharmacySource = "Community pharmacy",
		FluVaccinationReceiptNotes = "Demo vaccination record.",
		MaintenanceInhalerName = "Maintenance inhaler",
		MaintenanceInhalerDose = "1 puff",
		MaintenanceInhalerInstructions = "Demo treatment record. Confirm actual medication details with your clinician.",
		PrimaryCareCheckInTitle = "Primary care check-in",
		PrimaryCareCheckInClinician = "Dr. Rivera",
		DowntownClinicLocation = "Downtown Clinic",
		BringRecentLabResultNotes = "Bring recent lab result.",
		InfluenzaName = "Influenza",
		TakeMaintenanceInhalerTitle = "Take maintenance inhaler",
		ProfileDisplayName = "Demo vault"
	};
}

public static class DemoHealthVaultFactory
{
	public static HealthVaultSnapshot Create(DateTimeOffset now) =>
		Create(now, DemoHealthVaultText.English);

	internal static HealthVaultSnapshot Create(DateTimeOffset now, DemoHealthVaultText text)
	{
		var contact = new EmergencyContact
		{
			Id = Guid.Parse("0f28c8c2-8150-4a02-93c1-39a0a24b4f7d"),
			Name = text.EmergencyContactName,
			Relationship = text.EmergencyContactRelationship,
			PhoneNumber = "+1 555 010 0142",
			Notes = text.EmergencyContactNotes
		};

		var summary = new HealthSummary
		{
			BloodType = text.BloodType,
			Allergies = [text.Allergy],
			ChronicConditions = [text.ChronicCondition],
			Surgeries = [text.Surgery],
			EmergencyContacts = [contact],
			LastUpdatedAt = now.AddDays(-2)
		};

		var documents = new[]
		{
			new MedicalDocument
			{
				Id = Guid.Parse("45e4dcf3-4899-4e7f-a455-85d5b3e878cb"),
				Title = text.AnnualBloodPanelTitle,
				Category = DocumentCategory.LabResults,
				DocumentDate = DateOnly.FromDateTime(now.AddDays(-18).Date),
				Source = text.AnnualBloodPanelSource,
				Notes = text.AnnualBloodPanelNotes,
				Tags = ["blood", "annual"],
				IsDemo = true
			},
			new MedicalDocument
			{
				Id = Guid.Parse("db2e8730-6fa9-4775-a978-c2f0fbd5190e"),
				Title = text.AsthmaInhalerPrescriptionTitle,
				Category = DocumentCategory.Prescriptions,
				DocumentDate = DateOnly.FromDateTime(now.AddDays(-32).Date),
				Source = text.PrimaryCareSource,
				Notes = text.AsthmaInhalerPrescriptionNotes,
				Tags = ["asthma", "prescription"],
				IsDemo = true
			},
			new MedicalDocument
			{
				Id = Guid.Parse("08ebee47-cd87-4f2d-aee2-75774ad4ab74"),
				Title = text.FluVaccinationReceiptTitle,
				Category = DocumentCategory.Vaccinations,
				DocumentDate = DateOnly.FromDateTime(now.AddMonths(-4).Date),
				Source = text.CommunityPharmacySource,
				Notes = text.FluVaccinationReceiptNotes,
				Tags = ["flu", "vaccine"],
				IsDemo = true
			}
		};

		var schedule = new MedicationSchedule
		{
			Recurrence = ReminderRecurrence.Daily,
			TimeOfDay = new TimeOnly(8, 0),
			StartsOn = DateOnly.FromDateTime(now.AddDays(-20).Date)
		};

		var medications = new[]
		{
			new Medication
			{
				Id = Guid.Parse("6be3c297-fd55-4ec7-8c66-3ea11d668302"),
				Name = text.MaintenanceInhalerName,
				Dose = text.MaintenanceInhalerDose,
				Instructions = text.MaintenanceInhalerInstructions,
				Schedule = schedule,
				IsActive = true,
				RemindersEnabled = true
			}
		};

		var appointments = new[]
		{
			new Appointment
			{
				Id = Guid.Parse("f887f353-b8d2-497e-99ac-9084dfe12920"),
				Title = text.PrimaryCareCheckInTitle,
				Clinician = text.PrimaryCareCheckInClinician,
				Location = text.DowntownClinicLocation,
				StartsAt = now.AddDays(12).Date.AddHours(10),
				RelatedDocumentIds = [documents[0].Id],
				Notes = text.BringRecentLabResultNotes
			}
		};

		var vaccinations = new[]
		{
			new Vaccination
			{
				Id = Guid.Parse("2707e3f2-71a6-4fd8-a68c-63f0e1d5fb66"),
				Name = text.InfluenzaName,
				DateAdministered = DateOnly.FromDateTime(now.AddMonths(-4).Date),
				Provider = text.CommunityPharmacySource
			}
		};

		var reminders = new[]
		{
			new Reminder
			{
				Id = Guid.Parse("5efe669e-64c0-4191-a996-c7ac3519c20e"),
				Title = text.TakeMaintenanceInhalerTitle,
				StartsOn = DateOnly.FromDateTime(now.AddDays(-20).Date),
				TimeOfDay = new TimeOnly(8, 0),
				Recurrence = ReminderRecurrence.Daily
			}
		};

		var profile = new HealthProfile
		{
			Id = Guid.Parse("54b4f6a2-8d3b-47e7-bf8f-dc388f6d857f"),
			DisplayName = text.ProfileDisplayName,
			BloodType = summary.BloodType,
			Allergies = summary.Allergies,
			ChronicConditions = summary.ChronicConditions,
			Surgeries = summary.Surgeries,
			EmergencyContacts = summary.EmergencyContacts,
			LastUpdatedAt = summary.LastUpdatedAt
		};

		return new HealthVaultSnapshot
		{
			Profile = profile,
			Summary = summary,
			Documents = documents,
			Medications = medications,
			Appointments = appointments,
			Vaccinations = vaccinations,
			Reminders = reminders,
			VaultItems = CreateVaultItems(documents, medications, appointments, vaccinations, reminders, now),
			UpdatedAt = now
		};
	}

	private static IReadOnlyList<VaultItem> CreateVaultItems(
		IReadOnlyList<MedicalDocument> documents,
		IReadOnlyList<Medication> medications,
		IReadOnlyList<Appointment> appointments,
		IReadOnlyList<Vaccination> vaccinations,
		IReadOnlyList<Reminder> reminders,
		DateTimeOffset now) =>
		[
			.. documents.Select(document => new VaultItem
			{
				Id = document.Id,
				Kind = VaultItemKind.Document,
				Title = document.Title,
				UpdatedAt = now.AddDays(-1)
			}),
			.. medications.Select(medication => new VaultItem
			{
				Id = medication.Id,
				Kind = VaultItemKind.Medication,
				Title = medication.Name,
				UpdatedAt = now.AddDays(-2)
			}),
			.. appointments.Select(appointment => new VaultItem
			{
				Id = appointment.Id,
				Kind = VaultItemKind.Appointment,
				Title = appointment.Title,
				UpdatedAt = appointment.StartsAt
			}),
			.. vaccinations.Select(vaccination => new VaultItem
			{
				Id = vaccination.Id,
				Kind = VaultItemKind.Vaccination,
				Title = vaccination.Name,
				UpdatedAt = now.AddMonths(-4)
			}),
			.. reminders.Select(reminder => new VaultItem
			{
				Id = reminder.Id,
				Kind = VaultItemKind.Reminder,
				Title = reminder.Title,
				UpdatedAt = now.AddDays(-1)
			})
		];
}
