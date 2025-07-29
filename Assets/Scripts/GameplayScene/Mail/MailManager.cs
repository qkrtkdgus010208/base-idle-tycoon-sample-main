namespace Project.Gameplay
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using System.Collections.Generic;

    public class MailManager : MonoBehaviour
	{
        /// <summary>
        /// Mail data container
        /// </summary>
        [SerializeField] private SO_MailData _mailData;

        /// <summary>
        /// Mail UI
        /// </summary>
        [SerializeField] private GameObject _ui;

        /// <summary>
        /// Button to open Mail UI
        /// </summary>
        [SerializeField] private Button _openMailButton;

        /// <summary>
        /// Button to open Mail UI
        /// </summary>
        [SerializeField] private Button _closeMailButton;

        /// <summary>
        /// new mail indicator
        /// </summary>
        [SerializeField] private GameObject _newMailIndicator;

        /// <summary>
        /// Mail list ui template
        /// </summary>
        [SerializeField] private MailListUI _templateMailList;

        /// <summary>
        /// Mail list ui parent
        /// </summary>
        [SerializeField] private Transform _mailParentTransform;

        /// <summary>
        /// Refresh mail indicator
        /// </summary>
        [SerializeField] private GameObject _txtLoadingIndicator;

        /// <summary>
        /// Mail content UI
        /// </summary>
        [Space(40)]
        [Header("Mail Content UI")]
        [SerializeField] private GameObject _mailContentUI;

        /// <summary>
        /// current opened mail title text output
        /// </summary>
        [SerializeField] private TextMeshProUGUI _titleMailContentText;

        /// <summary>
        /// current opened mail content text output
        /// </summary>
        [SerializeField] private TextMeshProUGUI _msgMailContentText;

        /// <summary>
        /// button to claim current opened mail
        /// </summary>
        [SerializeField] private Button _claimMailContentButton;

        /// <summary>
        /// button to delete current opened mail
        /// </summary>
        [SerializeField] private Button _deleteMailContentButton;

        /// <summary>
        /// button to close current opened mail
        /// </summary>
        [SerializeField] private Button _closeMailContentButton;


        /// <summary>
        /// reward list template
        /// </summary>
        [SerializeField] private MailRewardItemsListUI _templateRewardList;

        /// <summary>
        /// reward list parent
        /// </summary>
        [SerializeField] private Transform _rewardListParent;

        /// <summary>
        /// List of mail indate that the ui mail already initialize 
        /// </summary>
        private List<string> mailInDateList = new List<string>();

        /// <summary>
        /// pool for reward list
        /// </summary>
        private Utility.ClassPooling<MailRewardItemsListUI> rewardListUIPool;

        /// <summary>
        /// Mail list ui that handle current opened mail
        /// </summary>
        private MailListUI currentOpenedMail;


        private void Awake()
        {
            _openMailButton.onClick.AddListener(() => SetActiveUI(true));
            _closeMailButton.onClick.AddListener(() => SetActiveUI(false));

            _claimMailContentButton.onClick.AddListener(ClaimMail);
            _deleteMailContentButton.onClick.AddListener(DeleteMail);
            _closeMailContentButton.onClick.AddListener(CloseMailContent);

            BackndServer.BackndNotification.OnReceiveMail += OnReceiveMail;

            rewardListUIPool = new Utility.ClassPooling<MailRewardItemsListUI>(
                () => Instantiate(_templateRewardList, _rewardListParent));

            FetchMail();
        }

        private void OnDestroy()
        {
            BackndServer.BackndNotification.OnReceiveMail -= OnReceiveMail;
        }

        /// <summary>
        /// Refresh mail from backnd console
        /// </summary>
        private void FetchMail()
        {
            _txtLoadingIndicator.SetActive(true);
            _mailData.RefreshMailData((mailDatas) =>
            {
                _newMailIndicator.SetActive(_mailData.MailsData.Count > 0);

                foreach (var mail in mailDatas)
                {
                    if (mailInDateList.Contains(mail.inDate))
                        continue;

                    var listUI = Instantiate(_templateMailList, _mailParentTransform);
                    listUI.SetMailElement(mail, OpenMail);
                    mailInDateList.Add(mail.inDate);
                }

                _txtLoadingIndicator.SetActive(false);
            });
        }

        /// <summary>
        /// Set active mail ui
        /// </summary>
        /// <param name="state"> true: set active / false: deactive </param>
        private void SetActiveUI(bool state)
        {
            if(!state)
                _newMailIndicator.SetActive(_mailData.MailsData.Count > 0);

            FetchMail();
            _ui.SetActive(state);
        }

        /// <summary>
        /// On receive new mail
        /// </summary>
        private void OnReceiveMail()
        {
            FetchMail();
            _newMailIndicator.SetActive(true);
        }

        /// <summary>
        /// Open mail content ui
        /// </summary>
        /// <param name="mailUI"> Mail list ui that handle current opened mail </param>
        private void OpenMail(MailListUI mailUI)
        {
            currentOpenedMail = mailUI;
            var mailData = currentOpenedMail.MailData;

            _titleMailContentText.SetText(mailData.title);
            _msgMailContentText.SetText(mailData.content);

            if (mailData.items != null)
            {
                foreach (var rewardItem in mailData.items)
                {
                    var listUI = rewardListUIPool.GetFromPool();
                    listUI.SetMailRewardElement(
                        Utility.StaticImageDictionary.Instance.GetImageSpriteByID(rewardItem.item.itemID), 
                        Utility.StaticCurrencyStringConverison.GetString(rewardItem.itemCount));
                    listUI.SetActive(true);
                }

                _claimMailContentButton.gameObject.SetActive(true);
            }
            else
                _claimMailContentButton.gameObject.SetActive(false);

            _deleteMailContentButton.gameObject.SetActive(mailData.postType == BackEnd.PostType.User);

            _closeMailButton.gameObject.SetActive(false);
            _mailContentUI.SetActive(true);
        }

        /// <summary>
        /// Close current opened mail
        /// </summary>
        private void CloseMailContent()
        {
            rewardListUIPool.ClearActiveObj();

            _closeMailButton.gameObject.SetActive(true);
            _mailContentUI.SetActive(false);
        }

        /// <summary>
        /// Claim current opened mail
        /// </summary>
        private void ClaimMail()
        {
            _mailData.ClaimMail(currentOpenedMail.MailData, (claimedItems) => {

                foreach (var claimedItem in claimedItems)
                {
                    switch (claimedItem.item.itemType)
                    {
                        case MailRewardCurrency.REWARD_TYPE_KEY:
                            new MailRewardCurrency(claimedItem.item.itemID, claimedItem.itemCount).ReceiveReward();
                            break;
                        case MailRewardLootbox.REWARD_TYPE_KEY:
                            new MailRewardLootbox(claimedItem.item.itemID, claimedItem.itemCount).ReceiveReward();
                            break;
                    }
                }

                currentOpenedMail.SetActiveUI(false);
                CloseMailContent();
            });
        }

        /// <summary>
        /// Delete current opened mail
        /// </summary>
        private void DeleteMail()
        {
            _mailData.DeleteMailData(currentOpenedMail.MailData, () =>
            {
                currentOpenedMail.SetActiveUI(false);
                CloseMailContent();
            });
        }
    }
}
