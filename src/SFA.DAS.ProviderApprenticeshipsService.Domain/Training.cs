using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain
{
    public class Training
    {
        public Training(TrainingType trainingType, string title, int id, int level, int pathwayCode)
        {
            TrainingType = trainingType;
            Title = title;
            Id = id;
            Level = level;
            PathwayCode = pathwayCode;
        }

        public TrainingType TrainingType { get; private set; }
        public string Title { get; private set; }
        public int Id { get; private set; }
        public int Level { get; private set; }
        public int PathwayCode { get; private set; }

        public string Code => TrainingType == TrainingType.Framework ? $"{Id}-{Level}-{PathwayCode}" : $"{Id}";

        public static Training Decode(string code, string title)
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                try
                {
                    var parts = code.Split(char.Parse("-"));

                    if (parts.Length == 1)
                    {
                        return new Training(TrainingType.Standard, title, int.Parse(code), 0, 0);
                    }
                    if (parts.Length == 3)
                    {
                        return new Training(TrainingType.Framework, title, int.Parse(parts[0]), int.Parse(parts[1]),
                            int.Parse(parts[2]));
                    }
                }
                catch (FormatException)
                {
                    //Let it fall through to the IOE below.
                }
            }

            throw new InvalidOperationException($"The code '{code}' is not valid");
        } 
    }
}