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
}
