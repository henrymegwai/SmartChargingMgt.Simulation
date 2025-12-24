namespace SmartChargingManagement.Api.Requests;

public record CreateConnectorRequest(int Id, int MaxCurrentInAmps, Guid ChargeStationId);

