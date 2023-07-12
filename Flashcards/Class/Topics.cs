﻿using System.Collections.Generic;

namespace Flashcards.Class
{
    public static class Topics
    {
        private static List<string> quotes = new List<string>() {
            "Age",
            "Alone",
            "Amazing",
            "Anger",
            "Anniversary",
            "Architecture",
            "Art",
            "Attitude",
            "Beauty",
            "Best",
            "Birthday",
            "Brainy",
            "Business",
            "Car",
            "Chance",
            "Change",
            "Christmas",
            "Communication",
            "Computers",
            "Cool",
            "Courage",
            "Dad",
            "Dating",
            "Death",
            "Design",
            "Diet",
            "Dreams",
            "Easter",
            "Education",
            "Environmental",
            "Equality",
            "Experience",
            "Failure",
            "Faith",
            "Family",
            "Famous",
            "Father's Day",
            "Fear",
            "Finance",
            "Fitness",
            "Food",
            "Forgiveness",
            "Freedom",
            "Friendship",
            "Funny",
            "Future",
            "Gardening",
            "God",
            "Good",
            "Government",
            "Graduation",
            "Great",
            "Happiness",
            "Health",
            "History",
            "Home",
            "Hope",
            "Humor",
            "Imagination",
            "Independence",
            "Inspirational",
            "Intelligence",
            "Jealousy",
            "Knowledge",
            "Leadership",
            "Learning",
            "Legal",
            "Life",
            "Love",
            "Marriage",
            "Medical",
            "Memorial Day",
            "Men",
            "Mom",
            "Money",
            "Morning",
            "Mother's Day",
            "Motivational",
            "Movies",
            "Moving On",
            "Music",
            "Nature",
            "New Year's",
            "Parenting",
            "Patience",
            "Patriotism",
            "Peace",
            "Pet",
            "Poetry",
            "Politics",
            "Positive",
            "Power",
            "Relationship",
            "Religion",
            "Respect",
            "Romantic",
            "Sad",
            "Saint Patrick's Day",
            "Science",
            "Smile",
            "Society",
            "Space",
            "Sports",
            "Strength",
            "Success",
            "Sympathy",
            "Teacher",
            "Technology",
            "Teen",
            "Thankful",
            "Thanksgiving",
            "Time",
            "Travel",
            "Trust",
            "Truth",
            "Valentine's Day",
            "Veterans Day",
            "War",
            "Wedding",
            "Wisdom",
            "Women",
            "Work"};
        private static List<string> popularQuotes = new List<string>()
        {
            "Art",
            "Friendship",
            "Life",
            "Love",
            "Motivational",
            "Success",
            "Wisdom"
        };
        private static List<string> idioms = new List<string>() {
                                                                "Business",
                                                                "Dog",
                                                                "Money",
                                                                "Love",
                                                                "Problem",
                                                                "Success",
                                                                };

        public static List<string> Quotes { get => quotes; set => quotes = value; }
        public static List<string> Idioms { get => idioms; set => idioms = value; }
        public static List<string> PopularQuotes { get => popularQuotes; set => popularQuotes = value; }
    }
}
