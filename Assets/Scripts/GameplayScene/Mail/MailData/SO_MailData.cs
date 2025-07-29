namespace Project.Gameplay
{
    using System;
    using UnityEngine;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    [CreateAssetMenu(fileName = "new data", menuName = "Scriptable Objects/Mail/Mail Data")]
    public class SO_MailData : ScriptableObject
    {
        /// <summary>
        /// List of mail data
        /// </summary>
        [SerializeField] private List<UPostItem> _mailsData = new List<UPostItem>();

        /// <summary>
        /// List of mail data
        /// </summary>
        public ReadOnlyCollection<UPostItem> MailsData => _mailsData.AsReadOnly();

        /// <summary>
        /// Refresh mail from backnd console
        /// </summary>
        /// <param name="onMailRefresh"></param>
        public void RefreshMailData(Action<List<UPostItem>> onMailRefresh)
        {
            BackndServer.BackndMail.LoadMailData(BackEnd.PostType.Admin,
                (json) =>
                {
                    _mailsData = Utility.StaticReflection.DatabaseItemsParse<UPostItem>(json);
                    onMailRefresh?.Invoke(_mailsData);
                });
        }

        /// <summary>
        /// Claim specific mail
        /// </summary>
        /// <param name="mailData"> mail data </param>
        /// <param name="onMailClaimed"> callback after successfull claim mail </param>
        public void ClaimMail(UPostItem mailData, Action<UPostItem.MailItemData[]> onMailClaimed)
        {
            onMailClaimed += (itemsID) => _mailsData.Remove(mailData);
            BackndServer.BackndMail.ClaimMail(mailData.postType, mailData.inDate,
                (json) =>
                {
                    var claimedItem = Utility.StaticReflection.DatabaseItemsParse<UPostItem.MailItemData>(json);
                    onMailClaimed?.Invoke(claimedItem.ToArray());
                });
        }

        /// <summary>
        /// Delete specific mail
        /// </summary>
        /// <param name="mailData"> mail data </param>
        /// <param name="onMailDeleted"> callback after successfull delete mail </param>
        public void DeleteMailData(UPostItem mailData, Action onMailDeleted)
        {
            onMailDeleted += () => _mailsData.Remove(mailData);
            BackndServer.BackndMail.DeleteMail(mailData.inDate, onMailDeleted);
        }

        /// <summary>
        /// Mail data
        /// </summary>
        [Serializable]
        public struct UPostItem
        {
            public BackEnd.PostType postType;
            public string title;
            public string content;
            public DateTime expirationDate;
            public DateTime reservationDate;
            public DateTime sentDate;
            public string nickname;
            public string inDate;
            public string author; // Exists only in admin mail
            public string rankType; // Exists only in ranking mail
            public List<MailItemData> items { get; set; }

            /// <summary>
            /// Mail reward item
            /// </summary>
            [Serializable]
            public struct MailItemData
            {
                public struct ItemData
                {
                    public string itemID;
                    public string itemType;
                }

                public ItemData item;
                public int itemCount;
                public string chartName;
            }
        }
    }
}
