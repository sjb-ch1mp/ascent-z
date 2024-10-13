using System;

public class ScoreManager
{
    // Multipliers
    int revivePenalty = 50;
    int survivorMultiplier = 100;

    // Score counters
    public Resources.Rank CurrentRank { get; set; }
    public int RankProgress { get; set; }
    public int KillScore { get; set; }
    public int SurvivorCount { get; set; }
    public int RevivesCount { get; set; }
    public void ResetScore() {
        KillScore = 0;
        SurvivorCount = 0;
        RevivesCount = 0;
    }
    
    public int GetFinalScore() {
        return  Math.Clamp(KillScore + (SurvivorCount * survivorMultiplier) - (RevivesCount * revivePenalty), 0, 999_999_999);
    }

    public int AddFinalScoreToRankProgress(int finalScore) {
        // Reach max rank - just ignore this call
        if (CurrentRank == Resources.Rank.FieldMarshal) {
            RankProgress = 0;
            return 0;
        }
        int increasedByRanks = 0;
        RankProgress += finalScore;
        if (RankProgress >= Resources.MAX_RANK_PROGRESS) {
            while (RankProgress >= Resources.MAX_RANK_PROGRESS) {
                increasedByRanks++;
                CurrentRank++;
                RankProgress -= Resources.MAX_RANK_PROGRESS;
                if (CurrentRank == Resources.Rank.FieldMarshal) {
                    break;
                }
            }
        }
        return increasedByRanks;
    }
}
