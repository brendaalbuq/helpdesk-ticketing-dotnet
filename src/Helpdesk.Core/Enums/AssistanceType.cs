//-----------------------------------------------------------------
//    <copyright file="AssistanceType.cs" company="IPCA">
//     Copyright IPCA-EST. All rights reserved.
//    </copyright>
//    <date>01-07-2026</date>
//    <time>19:40</time>
//    <version>1.0</version>
//    <author>Brenda Albuquerque</author>
//-----------------------------------------------------------------

namespace Helpdesk.Core.Enums
{
    /// <summary>
    /// Represents the type of assistance requested by the client.
    /// </summary>
    [CLSCompliant(true)]
    public enum AssistanceType
    {
        Hardware = 1,
        Software = 2,
        Network = 3,
        Warranty = 4,
        Account = 5,
        Other = 6
    }
}

