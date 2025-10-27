namespace Server
{
    public class Player
    {
        // player
        public Guid Id { get; set; } = Guid.NewGuid();
        public  float X { get; set; } // toa do goc trai x =0
        public  float Y { get; set; }// toa do goc trai y=0
        public float Rotation {  get; set; }
        public float Hp { get; set; }
        public int Ammo { get; set; } = 6;
        public float ShootDelay { get; set; } = 0.536f;
        public DateTime LastShootTime { get; set; }


        // dash skill
        public bool IsDashing { get; set; } = false;
        private float _dashForce { get; set; } = 50f;
        private float _timeDuration { get; set; } = 0.36f; // thoi gian ton tai dash
        private float _dashCooldown { get; set; } = 3.6f;
        private float _dashTimeLeft { get; set; } = 0f;
        private float _cooldownLeft { get; set; } = 0f;
        private float _dashDirection { get; set; }

        // 
        public bool IsUltimateActive { get; set; } = false;
        public DateTime UltimateStartFire { get; set; }
        public float UltimateDuration {  get; set; } = 1f;
        public float UltimateFireRate { get; set; } = 0.2f;
        public DateTime LastUltimateFireTime { get; set; }// kiem tra xem khoang cach giua 2 lan xa dan
        public DateTime LastUltimateTime {  get; set; } = DateTime.Now; // + voi cool down thanh hoi chieu
        public float UltimateCooldown { get; set; } = 10f;

        //sprite
        public float Width { get; set; }
        public float Height { get; set; }

        // die anh rebirth 
        public PlayerState State { get; set; } = PlayerState.Alive;
        public float RespawnTime { get; set; } = 5f;



        public bool AllowShoot()
        {
            if (Ammo <= 0)
            {
                return false;
            }
            if((DateTime.Now- LastShootTime).TotalSeconds < ShootDelay)
            {
                return false;
            }
            Ammo--;
            LastShootTime = DateTime.Now;
            return true;
        }
        public void GunReload()
        {
            Ammo = 6;
        }// done
        public void Die()
        {
            if (State == PlayerState.Alive)
            {
                State = PlayerState.Dead;
                Hp = 0;
                RespawnTime = 5f;
            }
        }//done
        public void StartDash()
        {
            if (State == PlayerState.Dead) return;
            if (_cooldownLeft>0f) // chua hoi chieu xong
            {
                return;
            }
            if (IsDashing) return; // neu dang dash thi k cho dash nua

            IsDashing = true;
            _dashTimeLeft = _timeDuration;
            _cooldownLeft = _dashCooldown;
            _dashDirection = Rotation;
        }// done
        public void DashSkill(float deltaTime)
        {
            if (_cooldownLeft > 0f)
            {
                _cooldownLeft -= deltaTime;
            }
            if (IsDashing)
            {
                _dashTimeLeft-= deltaTime;
                MoveForward(_dashForce );
                if (_dashTimeLeft < 0f)
                {
                    IsDashing = false; // het dash
                }
            }
        }// done
        public void  MoveForward(float distance)
        {
            float rad = (float)(_dashDirection* Math.PI / 180); //deg to rad
            X += (float)(MathF.Cos(rad) * distance);
            Y += (float)(MathF.Sin(rad) * distance);
        }// done

        public void StartSkillUltimate()
        {
            var now= DateTime.Now;
            if (IsUltimateActive) return;
            if ((now-LastUltimateTime).TotalSeconds<UltimateCooldown)
            {
                return;
            }
            IsUltimateActive = true;
            LastUltimateTime = now;
            UltimateStartFire = now;
        }
    }

    public enum PlayerState
    {
        Alive,
        Dead 
    }// done

}
