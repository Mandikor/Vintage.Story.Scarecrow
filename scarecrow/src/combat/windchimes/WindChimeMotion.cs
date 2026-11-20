using System;

namespace Scarecrow
{
    /// <summary>Client-local, damped wind response. Each arm owns its own phase and spring state.</summary>
    internal sealed class WindChimeMotion
    {
        private double pitch, roll, pitchVelocity, rollVelocity, time;
        private readonly double phase;

        public WindChimeMotion(long entityId, int arm)
        {
            // Stable variation without consuming the world's random stream.
            phase = ((unchecked((ulong)entityId * 11400714819323198485UL) ^ (ulong)(arm + 1) * 2654435761UL) % 10000) * .00062831853;
        }

        public void Advance(float dt, double windX, double windZ)
        {
            if (!float.IsFinite(dt) || dt <= 0) return;
            if (!double.IsFinite(windX) || !double.IsFinite(windZ)) windX = windZ = 0;
            // Bound catch-up and integrate in small steps so lag cannot explode the spring.
            double remaining = Math.Min(dt, .1);
            while (remaining > 0)
            {
                double step = Math.Min(remaining, 1d / 120);
                time += step;
                double gust = .7 + .2 * Math.Sin(time * 1.9 + phase) + .1 * Math.Sin(time * .9);
                Spring(ref pitch, ref pitchVelocity, Math.Clamp(windZ * gust * 21, -35, 35), step);
                Spring(ref roll, ref rollVelocity, Math.Clamp(-windX * gust * 21, -35, 35), step);
                remaining -= step;
            }
        }

        private static void Spring(ref double angle, ref double velocity, double target, double dt)
        {
            const double frequency = 7.5, damping = .4;
            velocity += (frequency * frequency * (target - angle) - 2 * damping * frequency * velocity) * dt;
            angle = Math.Clamp(angle + velocity * dt, -50, 50);
        }

        public (float pitch, float yaw, float roll) GetAngles(int part)
        {
            double offset = phase + part * 2.39996323;
            double strength = Math.Min(1, Math.Sqrt(pitch * pitch + roll * roll) / 15);
            double amplitude = .35 + 4 * strength;
            double oscillation = time * (1.2 + (part % 5) * .19) + offset;
            return ((float)Math.Clamp(pitch + amplitude * Math.Sin(oscillation), -50, 50),
                (float)(Math.Sin(time * .8 + offset) * (1.5 + 7 * strength)),
                (float)Math.Clamp(roll + amplitude * Math.Cos(oscillation * .93), -50, 50));
        }
    }
}
