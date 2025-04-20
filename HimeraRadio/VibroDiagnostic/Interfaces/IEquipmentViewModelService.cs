using VibroDiagnostic.ViewModels;
using VibroDiagnostic.ViewModels.Equipment;

namespace VibroDiagnostic.Interfaces;

public interface IEquipmentViewModelService
{
    Task<EquipmentViewModelIndex> GetAllEquipmentsAsync();
    Task<EquipmentViewModel> GetEquipmentByIdAsync(int id);
    
    Task<EquipmentViewModel> UpsertAsync(EquipmentViewModel equipment);
}