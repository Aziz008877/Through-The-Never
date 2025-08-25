public interface IDotReceivable
{
    bool  IsDotActive { get; set; }
    void  ApplyDot(float dps, float duration, float tickRate = 1f);
    void  RefreshDot(float duration);
}