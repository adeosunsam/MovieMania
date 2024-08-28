using System;
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

}