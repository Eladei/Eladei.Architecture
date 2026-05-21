namespace Eladei.BookRating.Api.Helpers;

/// <summary>
/// Helper class for working with external variables stored in a .env file
/// </summary>
public static class EnvVariablesHelper
{
    /// <summary>
    /// Retrieves a variable from the .env file
    /// </summary>
    /// <typeparam name="T">Type of the variable</typeparam>
    /// <param name="envVariableName">Name of the environment variable</param>
    /// <returns>Value of the environment variable</returns>
    /// <exception cref="ArgumentNullException">Thrown when the variable is not found</exception>
    /// <exception cref="NotImplementedException">Thrown when the requested type is not supported</exception>
    public static T GetVariable<T>(string envVariableName)
    {
        var envVariable = Environment.GetEnvironmentVariable(envVariableName)
            ?? throw new ArgumentNullException(envVariableName);

        Type resultType = typeof(T);

        return resultType switch
        {
            Type t when t == typeof(string) => (T)(object)envVariable,
            Type t when t == typeof(int) => (T)(object)Convert.ToInt32(envVariable),
            Type t when t == typeof(bool) => (T)(object)Convert.ToBoolean(envVariable),
            Type t when t == typeof(double) => (T)(object)Convert.ToDouble(envVariable),
            Type t when t == typeof(float) => (T)(object)Convert.ToSingle(envVariable),
            Type t when t == typeof(long) => (T)(object)Convert.ToInt64(envVariable),
            Type t when t == typeof(short) => (T)(object)Convert.ToInt16(envVariable),
            Type t when t == typeof(decimal) => (T)(object)Convert.ToDecimal(envVariable),
            Type t when t == typeof(DateTime) => (T)(object)DateTime.Parse(envVariable),
            Type t when t == typeof(byte) => (T)(object)Convert.ToByte(envVariable),
            Type t when t == typeof(sbyte) => (T)(object)Convert.ToSByte(envVariable),
            Type t when t == typeof(char) => (T)(object)Convert.ToChar(envVariable),
            Type t when t == typeof(uint) => (T)(object)Convert.ToUInt32(envVariable),
            Type t when t == typeof(ulong) => (T)(object)Convert.ToUInt64(envVariable),
            Type t when t == typeof(ushort) => (T)(object)Convert.ToUInt16(envVariable),
            _ => throw new NotImplementedException()
        };
    }
}