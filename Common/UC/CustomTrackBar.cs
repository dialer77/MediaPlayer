using System.Drawing;
using System.Windows.Forms;

namespace Common.UC
{
    public class CustomTrackBar : TrackBar
    {
        public CustomTrackBar()
        {
            // 기본 설정
            SetStyle(ControlStyles.UserPaint, true); // 사용자 정의 그리기를 허용
        }

        // 사용자 정의 그리기
        protected override void OnPaint(PaintEventArgs e)
        {
            // 기본 그래픽을 사용하지 않음
            e.Graphics.Clear(this.BackColor); // 배경색 설정

            // 슬라이더 그리기
            DrawTrack(e.Graphics);

            // Thumb (핸들) 그리기
            DrawThumb(e.Graphics);
        }

        private void DrawTrack(Graphics g)
        {
            // 트랙(슬라이드 바) 그리기
            int trackHeight = 4;
            int trackY = (this.Height - trackHeight) / 2; // 가운데 정렬

            using (Brush brush = new SolidBrush(Color.LightGray))
            {
                g.FillRectangle(brush, 0, trackY, this.Width, trackHeight);
            }
        }

        private void DrawThumb(Graphics g)
        {
            // Thumb(핸들) 그리기
            int thumbSize = 20;
            int thumbX = (int)((float)(this.Value - this.Minimum) / (this.Maximum - this.Minimum) * (this.Width - thumbSize));

            using (Brush brush = new SolidBrush(Color.Blue))
            {
                g.FillEllipse(brush, thumbX, (this.Height - thumbSize) / 2, thumbSize, thumbSize);
            }
        }
    }
}

       
	
