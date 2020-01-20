using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;

namespace SHP.Modelo.SP2016.Features.Backend
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("2dcd82df-e268-4fa6-aef9-a1139f92ac6b")]
    public class BackendEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        //public override void FeatureActivated(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}

        public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, IDictionary<string, string> parameters)
        {
            var parentWeb = (SPWeb)properties.Feature.Parent;
            if (parentWeb == null) return;

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (var elevatedSite = new SPSite(parentWeb.ID))
                {
                    using (var elevatedWeb = elevatedSite.OpenWeb(elevatedSite.ID))
                    {
                        try
                        {
                            switch (upgradeActionName)
                            {
                                case "V2Update":
                                    parentWeb.AllowUnsafeUpdates = true;

                                    foreach (string key in parameters.Keys)
                                    {
                                        // Iterate and split each content type, field value.
                                        string value = parameters[key];

                                        string[] parts = value.Split(',');

                                        SPContentTypeId contentTypeId = new SPContentTypeId(parts[0]);
                                        Guid fieldId = new Guid(parts[1]);

                                        SPField field = parentWeb.Fields[fieldId];
                                        SPFieldLink fieldLink = new SPFieldLink(field);
                                        SPContentType contentType = parentWeb.ContentTypes[contentTypeId];

                                        //Logger.Information("Adding field \"{0}\" to content type \"{1}\".", field.Title, contentType.Name);

                                        contentType.FieldLinks.Delete(fieldId);

                                        contentType.Update(true);

                                        contentType.FieldLinks.Add(fieldLink);

                                        contentType.Update(true);
                                    }
                                    parentWeb.AllowUnsafeUpdates = false;
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            // log error
                            //ogger.Unexpected("GCS.NovosCanais__BrandingEventReceiver.FeatureUpgrading", ex);
                            throw;
                        }
                    }
                }
            });


        }
    }
}
