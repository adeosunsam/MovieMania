using System;
using System.ComponentModel.DataAnnotations;
using UnityEngine;

public class RequestDtos
{
    [Serializable]
    public class UserDetailDto
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public Sprite Sprite { get; set; }
        public bool IsImageLoadingStopped { get; set; }
        public string Image { get; set; }
    }

    public class UserFollowRequest
    {
        public string UserId { get; set; }
        public string FriendId { get; set; }
    }

    public class ManageFriendRequest
    {
        public string UserId { get; set; }
        public string FriendId { get; set; }
        public ManageFriend Action { get; set; }
    }

    public enum ManageFriend
    {
        Accept = 1,
        Decline
    }
}