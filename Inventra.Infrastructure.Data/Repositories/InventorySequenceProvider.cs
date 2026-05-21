using System.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Inventra.Infrastructure.Data.Repositories;

internal class InventorySequenceProvider(AppDbContext dbContext) : IInventorySequenceProvider
{
    public async Task<long> GetNextSequenceAsync(
        Guid inventoryId,
        CancellationToken cancellationToken = default)
    {
        var shouldClose = dbContext.Database.GetDbConnection().State != ConnectionState.Open;

        if (shouldClose)
            await dbContext.Database.OpenConnectionAsync(cancellationToken);

        try
        {
            return await ExecuteNextSequenceCommandAsync(inventoryId, cancellationToken);
        }
        finally
        {
            if (shouldClose)
                await dbContext.Database.CloseConnectionAsync();
        }
    }

    private async Task<long> ExecuteNextSequenceCommandAsync(
        Guid inventoryId,
        CancellationToken cancellationToken)
    {
        await using var command = CreateCommand(inventoryId);
        var value = await command.ExecuteScalarAsync(cancellationToken);

        return Convert.ToInt64(value);
    }

    private System.Data.Common.DbCommand CreateCommand(Guid inventoryId)
    {
        var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.Transaction = dbContext.Database.CurrentTransaction?.GetDbTransaction();
        command.CommandText = NextSequenceSql();
        command.Parameters.Add(CreateInventoryIdParameter(command, inventoryId));

        return command;
    }

    private static System.Data.Common.DbParameter CreateInventoryIdParameter(
        System.Data.Common.DbCommand command,
        Guid inventoryId)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = "inventoryId";
        parameter.Value = inventoryId;

        return parameter;
    }

    private static string NextSequenceSql()
    {
        return """
               INSERT INTO inventory_sequences (inventory_id, next_value)
               VALUES (@inventoryId, 2)
               ON CONFLICT (inventory_id)
               DO UPDATE SET next_value = inventory_sequences.next_value + 1
               RETURNING inventory_sequences.next_value - 1
               """;
    }
}
