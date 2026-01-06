using UnityEngine;

public class EffectController : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private int lastEffect = 1;



    private enum State
    {
        Off,
        Playing,
        Stopping
    }

    private State state = State.Off;

    void Update()
    {
        if (state == State.Stopping)
        {
            // 아직 살아있는 입자가 있으면 대기
            if (ps.IsAlive(true))
                return;

            // 전부 사라졌으면 완전 종료
            ps.gameObject.SetActive(false);
            state = State.Off;
        }
    }

    /// 공격 시작 시 호출
    public void Play()
    {
        if (state == State.Playing) return;

        ps.gameObject.SetActive(true);
        ps.Play(true);
        state = State.Playing;
    }

    /// 공격 종료 시 호출
    public void StopEffect()
    {
        if (state != State.Playing) return;

        // 마지막 파티클 방출
        if (lastEffect > 0)
            ps.Emit(lastEffect);

        // 방출만 중단 (기존 입자는 유지)
        ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        state = State.Stopping;
    }
}
