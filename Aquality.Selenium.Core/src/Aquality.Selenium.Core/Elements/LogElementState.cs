namespace Aquality.Selenium.Core.Elements
{
    /// <summary>
    /// Logs element state.
    /// </summary>
    /// <param name="messageKey">Key of localized message to log.</param>
    /// <param name="stateKey">Key of localized state to log.</param>
    public delegate void LogElementState(string messageKey, string stateKey);
}
