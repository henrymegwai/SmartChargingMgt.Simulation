namespace SmartChargingManagement.Api.Requests;

public record CreateConnectorRequest(int MaxCurrentInAmps, Guid ChargeStationId);

