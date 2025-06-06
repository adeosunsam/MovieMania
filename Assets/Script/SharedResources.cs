using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using static QuestionDto;
using static RequestDtos;
using static ResponseDtos;

public class SharedResources
{
    internal static void StartUp()
    {
        Debug.LogWarning($"TopicResponse Count in shared:{TopicResponse?.Count}");
        TopicResponse.ForEach(async x =>
        {   
            x.Sprite = await LoadImageAsync(x.Image);
        });

        /*playerActivity.ForEach(async x =>
        {
            x.Sprite = await LoadTopicImageAsync(x.ProfilePicture);
        });*/
    }

    internal static void LoadUserImages()
    {
        Debug.LogWarning($"UserFriend Count in shared:{UserFriends?.Count}");
        UserFriends?.ForEach(async x =>
        {
            if (!string.IsNullOrWhiteSpace(x.Image))
            {
                x.Sprite = await LoadImageAsync(x.Image);
            }
        });
    }

    internal static void LoadUserSearchImages()
    {
        Debug.LogWarning($"User Search Count in shared:{UserSearchDetails?.Count}");
        UserSearchDetails?.ForEach(async x =>
        {
            if (!string.IsNullOrWhiteSpace(x.Image))
            {
                x.Sprite = await LoadImageAsync(x.Image);
            }
            x.IsImageLoadingStopped = true;
        });
    }

    internal static async Task<Sprite> LoadImageAsync(string base64)
    {
        try
        {
            var imagebyte = await Task.Run(() => Convert.FromBase64String(base64));

            if (imagebyte != null)
            {
                var tcs = new TaskCompletionSource<Sprite>();

                MainThreadDispatcher.Enqueue(() =>
                {
                    var texture = LoadTextureFromByteArray(imagebyte);
                    Sprite sprite = SpriteFromTexture2D(texture);
                    tcs.SetResult(sprite);
                });
                return await tcs.Task;
            }
            else
            {
                Debug.LogError("unable to load base 64 string");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        return null;
    }

    internal static IEnumerator LoadProfilePics(UserDetailDto request)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(request.Image))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError
                || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                request.IsImageLoadingStopped = true;
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);

                request.Sprite = SpriteFromTexture2D(texture);
            }
        }
    }

    internal static UserDetailDto UserDetail;
    internal static List<UserDetailDto> UserSearchDetails;
    internal static UserDetailResponseDto OpponentDetail;
    internal static UserGamingCountDto GamingCount;
    internal static List<TopicResponseDto> TopicResponse;

    internal static ICollection<Question> Questions;
    internal static TopicResponseDto TopicInPlay;
    internal static List<UserActivityResponse> UserActivity;
    internal static List<UserDetailResponseDto> UserFriends;

    private static Texture2D LoadTextureFromByteArray(byte[] imageBytes)
    {
        try
        {
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageBytes))
            {
                return texture;
            }
            Debug.LogError("Failed to load texture from base64 string.");
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Exception =====> {ex.Message}");
        }
        return null;
    }

    internal static Sprite SpriteFromTexture2D(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public class FollowedTopic
    {
        public string UserId { get; set; }
        public string TopicId { get; set; }

        public FollowedTopic(string topicId)
        {
            TopicId = topicId;
        }
    }
}
