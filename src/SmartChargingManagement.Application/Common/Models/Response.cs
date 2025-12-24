using FluentResults;

namespace SmartChargingManagement.Application.Common.Models;

public record Response<T>(bool Status, T Data, string Message, Result Error = null!);