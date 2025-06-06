using System;
using UnityEngine;

public class ResponseDtos
{
    public class TopicResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Sprite Sprite { get; set; }
        public string Category { get; set; }
        public string Image { get; set; } // in base64
        public int QuestionCount { get; set; }
        public int FollowersCount { get; set; }
        public bool IsFollowed { get; set; }
    }

    public class UserGamingCountDto
    {
        public string Id { get; set; }
        public int TotalGamePlayed { get; set; }
        public int TotalFriends { get; set; }
    }

    public class UserDetailResponseDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Image { get; set; }//image in base64
        public Sprite Sprite { get; set; }
    }

    public class UserActivityResponse
    {
        public Sprite Sprite { get; set; }
        public string Id { get; set; }
        public string SenderName { get; set; }
        public string SenderId { get; set; }
        public string UserImage { get; set; }
        public ActivityEnum Activity { get; set; }
        public string TopicName { get; set; }
        public string TopicId { get; set; }
        public string GroupId { get; set; }

        public enum ActivityEnum
        {
            Challenge = 1,
            Follow
        }
    }
}
