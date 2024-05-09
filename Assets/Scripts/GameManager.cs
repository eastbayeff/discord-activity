using System.Collections;
using UnityEngine;
using static Dissonity.Api;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.UI;
using Dissonity;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerUiPrefab;
    [SerializeField] private TMP_Text playerCountText;
    [SerializeField] private TMP_Text serverUpdateText;

    [SerializeField] private Transform playerUiContainer;

    private int lastParticipantCount;

    private const string DISCORD_AVATAR_URL = "https://cdn.discordapp.com/avatars/{0}/{1}.png?size=256";


    async void Start()
    {
        var userId = await GetUserId();
        DissonityLog("User ID: " + userId);

        var user = await GetUser();
        DissonityLog("Username: " + user.username);
        DissonityLog("Display_name: " + user.display_name);
        DissonityLog("User Avatar: " + user.avatar);
        serverUpdateText.text = "User: " + user.username;

        // StartCoroutine(CreateImagePrefab(user));

        //var instanceParticipants = await GetInstanceParticipants();
        //lastParticipantCount = instanceParticipants.participants.Length;
        //playerCountText.text = lastParticipantCount.ToString();
        //serverUpdateText.text = "Subscribing to Participants Update";
        //await UpdateParticipantImagesAsync(instanceParticipants.participants);

        //SubActivityInstanceParticipantsUpdate(async (data) =>
        //{
        //    if (data.participants.Length < lastParticipantCount)
        //    {
        //        lastParticipantCount = data.participants.Length;
        //        serverUpdateText.text = "Player left";
        //        playerCountText.text = lastParticipantCount.ToString();
        //    }
        //    else if (data.participants.Length > lastParticipantCount)
        //    {
        //        lastParticipantCount = data.participants.Length;
        //        serverUpdateText.text = "Player joined";
        //        playerCountText.text = lastParticipantCount.ToString();
        //    }

        //    await UpdateParticipantImagesAsync(data.participants);
        //});
    }


    async Task UpdateParticipantImagesAsync(Participant[] participants)
    {
        DissonityLog("Updating participant images");
        while (playerUiContainer.childCount > 0)
        {
            Destroy(playerUiContainer.GetChild(0).gameObject);
        }

        foreach (var player in participants)
        {
            var playerImage = await GetSpriteForParticipantAsync(player);
            var clone = Instantiate(playerUiPrefab, playerUiContainer);
            clone.GetComponent<Image>().sprite = playerImage;
            DissonityLog("Updating participant images");
        }
    }

    IEnumerator CreateImagePrefab(User user)
    {
        var url = string.Format(DISCORD_AVATAR_URL, user.id, user.avatar);
        DissonityLog("Image URL requested: " + url);
        using UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

        yield return www.SendWebRequest();

        var userPrefab = Instantiate(playerUiPrefab, playerUiContainer);
        var userImage = Sprite.Create(DownloadHandlerTexture.GetContent(www), new Rect(0, 0, 256, 256), new Vector2(0.5f, 0.5f));
        userPrefab.GetComponent<Image>().sprite = userImage;
    }


    async Task<Sprite> GetSpriteForParticipantAsync(User user)
    {
        var url = string.Format(DISCORD_AVATAR_URL, user.id, user.avatar);
        DissonityLog("Image URL requested: " + url);
        using UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        var asyncOp = www.SendWebRequest();

        while (!asyncOp.isDone)
        {
            await Task.Delay(1000 / 30).ConfigureAwait(false);
            DissonityLog("Downloading image...");
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            DissonityWarn(www.error);
            return null;
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            if (texture != null)
            {
                DissonityLog("Texture found, creating sprite");
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                DissonityLog("Failed to load texture from " + url);
                return null;
            }
        }
    }

}
