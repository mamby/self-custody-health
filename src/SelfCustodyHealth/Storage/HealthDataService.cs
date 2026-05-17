using System.Globalization;
using SelfCustodyHealth.Domain;
using SelfCustodyHealth.Shared.Localization;

namespace SelfCustodyHealth.Storage;

public sealed class HealthDataService(IVaultStore vaultStore)
{
	private readonly SemaphoreSlim _gate = new(1, 1);
	private HealthVaultSnapshot? _snapshot;
	private string? _demoCultureName;

	public bool IsDemoData { get; private set; } = true;

	public async Task<HealthVaultSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default)
	{
		await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
		try
		{
			if (_snapshot is not null && (!IsDemoData || _demoCultureName == CultureInfo.CurrentUICulture.Name))
			{
				return _snapshot;
			}

			var saved = await vaultStore.LoadAsync(cancellationToken).ConfigureAwait(false);
			if (saved is not null)
			{
				IsDemoData = false;
				_snapshot = saved;
				return saved;
			}

			IsDemoData = true;
			_snapshot = CreateDemoSnapshot();
			return _snapshot;
		}
		finally
		{
			_gate.Release();
		}
	}

	public async Task SaveAsync(HealthVaultSnapshot snapshot, CancellationToken cancellationToken = default)
	{
		await vaultStore.SaveAsync(snapshot, cancellationToken).ConfigureAwait(false);
		_snapshot = snapshot;
		IsDemoData = false;
	}

	public async Task DeleteLocalVaultAsync(CancellationToken cancellationToken = default)
	{
		await vaultStore.DeleteAsync(cancellationToken).ConfigureAwait(false);
		_snapshot = CreateDemoSnapshot();
		IsDemoData = true;
	}

	private HealthVaultSnapshot CreateDemoSnapshot()
	{
		_demoCultureName = CultureInfo.CurrentUICulture.Name;
		return DemoHealthVaultFactory.Create(DateTimeOffset.Now, LocalizedDemoHealthVaultText.Create());
	}
}
