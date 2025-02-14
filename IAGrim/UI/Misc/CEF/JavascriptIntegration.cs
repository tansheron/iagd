﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using IAGrim.Database.Model;
using IAGrim.Settings.Dto;
using IAGrim.Utilities;
using Newtonsoft.Json;

// Methods are called from Javascript, Resharper cannot detect usage.
// ReSharper disable UnusedMember.Global

namespace IAGrim.UI.Misc.CEF {
    public class JavascriptIntegration {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Culture = System.Globalization.CultureInfo.InvariantCulture,
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public event EventHandler ItemTransferEvent;
        public event EventHandler OnClipboard;
        public event EventHandler OnRequestItems;
        public event EventHandler OnRequestSetItemAssociations;
        public event EventHandler OnRequestBackedUpCharacterList;
        public event EventHandler OnRequestCharacterDownloadUrl;

        public string TransferItem(object[] identifier, bool transferAll) {
            var args = new StashTransferEventArgs(identifier, transferAll);
             
            ItemTransferEvent?.Invoke(this, args);

            // Frontend expects a reply on success/failure
            var ret = new Dictionary<string, object> {
                {"success", args.IsSuccessful},
                {"numTransferred", args.NumTransferred}
            };

            return JsonConvert.SerializeObject(ret, _settings);
        }

        public string GetTranslationStrings() {
            var lang = RuntimeSettings.Language;
            Dictionary<string, string> translations = new Dictionary<string, string> {
                {"app.tab.items", lang.GetTag("iatag_html_tab_header_items")},
                {"app.tab.collections", lang.GetTag("iatag_html_tab_header_collections")},
                {"app.tab.help", lang.GetTag("iatag_html_tab_header_help")},
                {"app.tab.crafting", lang.GetTag("iatag_html_tab_header_crafting")},
                {"app.tab.components", lang.GetTag("iatag_html_tab_header_components")},
                {"app.tab.videoGuide", lang.GetTag("iatag_html_tab_header_videoguide")},
                {"app.tab.videoGuideUrl", lang.GetTag("iatag_html_tab_header_videoguide_url")},
                {"app.tab.discord", lang.GetTag("iatag_html_tab_header_discord")},
                {"items.label.noItemsFound", lang.GetTag("iatag_html_items_no_items")},
                {"items.label.youCanCraftThisItem", lang.GetTag("iatag_html_items_youcancraftthisitem")},
                {"items.label.cloudOk", lang.GetTag("iatag_html_cloud_ok")},
                {"items.label.cloudError", lang.GetTag("iatag_html_cloud_err")},
                {"items.label.cloudOk.hw", lang.GetTag("iatag_html_cloud_ok_hw")},
                {"items.label.cloudError.hw", lang.GetTag("iatag_html_cloud_err_hw")},
                {"items.label.cloudOk.easter", lang.GetTag("iatag_html_cloud_ok_easter")},
                {"items.label.cloudError.easter", lang.GetTag("iatag_html_cloud_err_easter")},
                {"item.buddies.singular", lang.GetTag("iatag_html_items_buddy_alsohasthisitem1")},
                {"item.buddies.plural", lang.GetTag("iatag_html_items_buddy_alsohasthisitem3")},
                {"item.buddies.singularOnly", lang.GetTag("iatag_html_items_buddy_alsohasthisitem4")},
                {"items.label.doubleGreen", lang.GetTag("iatag_html_items_affix2")},
                {"items.label.tripleGreen", lang.GetTag("iatag_html_items_affix3")},
                {"item.label.bonusToAllPets", lang.GetTag("iatag_html_bonustopets")},
                {"item.label.grantsSkill", lang.GetTag("iatag_html_items_grantsskill")},
                {"item.label.grantsSkillLevel", lang.GetTag("iatag_html_items_level")},
                {"item.label.levelRequirement", lang.GetTag("iatag_html_levlerequirement")},
                {"item.label.levelRequirementAny", lang.GetTag("iatag_html_any")},
                {"item.label.transferSingle", lang.GetTag("iatag_html_transfer")},
                {"item.buddies.tooltip", lang.GetTag("iatag_html_buddies_tooltip")},
                {"item.label.transferCompareSingle", lang.GetTag("iatag_html_transfer_cmp")},
                {"item.label.transferAll", lang.GetTag("iatag_html_transferall")},
                {"crafting.header.recipeName", lang.GetTag("iatag_html_badstate_title")},
                {"crafting.header.currentlyLacking", lang.GetTag("iatag_html_crafting_lacking")},
                {"item.augmentPurchasable", lang.GetTag("iatag_html_augmentation_item")},
                {"app.copyToClipboard", lang.GetTag("iatag_html_copytoclipboard")},
                {"item.label.setbonus", lang.GetTag("iatag_html_setbonus")},
                {"item.label.noMoreItems", lang.GetTag("iatag_html_nomoreitems")},
                {"item.label.setConsistsOf", lang.GetTag("iatag_html_setconsistsof") },
                {"button.loadmoreitems", lang.GetTag("iatag_html_loadmoreitems") },

                {"items.displaying", lang.GetTag("iatag_html_displaying") },
                {"collections.filter.owned", lang.GetTag("iatag_html_filter_owned") },
                {"collections.filter.missing", lang.GetTag("iatag_html_filter_missing") },
                {"collections.filter.purple", lang.GetTag("iatag_html_filter_purple")},
                {"collections.h2", lang.GetTag("iatag_html_h2") },
                {"collections.ingress1", lang.GetTag("iatag_html_ingress1") },
                {"collections.ingress2", lang.GetTag("iatag_html_ingress2") },
                {"notification.clearall", lang.GetTag("iatag_html_clearall")},
                {"app.error.grimnotparsed", lang.GetTag("iatag_html_grimnotparsed")},

                {"item.genericstats.watermark", lang.GetTag("iatag_html_genericstats")},
                {"item.buddies.watermark", lang.GetTag("iatag_html_buddies_watermark")},
            };

            // Attempting to return a Dictionary<..> object will only work if this object is bound with "async: true"
            return JsonConvert.SerializeObject(translations, _settings);
        }


        public void SetClipboard(string data) {
            if (!string.IsNullOrWhiteSpace(data)) {
                OnClipboard?.Invoke(this, new ClipboardEventArg {Text = data});
            }
        }

        // TODO: Weird flow, should just return items.
        public void RequestMoreItems() {
            OnRequestItems?.Invoke(this, null);
        }

        public string GetBackedUpCharacters() {
            RequestCharacterListEventArg args = new RequestCharacterListEventArg();
            OnRequestBackedUpCharacterList?.Invoke(this, args);
            return JsonConvert.SerializeObject(args.Characters, _settings);
        }

        public string GetCharacterDownloadUrl(string c) {
            RequestCharacterDownloadUrlEventArg args = new RequestCharacterDownloadUrlEventArg {
                Character = c
            };

            OnRequestCharacterDownloadUrl?.Invoke(this, args);
            return JsonConvert.SerializeObject(args, _settings);
        }

        public string GetItemSetAssociations() {
            GetSetItemAssociationsEventArgs args = new GetSetItemAssociationsEventArgs();
            OnRequestSetItemAssociations?.Invoke(this, args);
            return JsonConvert.SerializeObject(args.Elements, _settings);
        }
    }
}