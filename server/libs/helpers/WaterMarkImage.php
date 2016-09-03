<?php

class WaterMarkImage {

    public static function getImage ($image, $type, $pos) {
        $type = strip_tags($type); //тип (новинка или лидер)
        if((int)$type === 1)
        {
            $imgM = "images/site/novinka.png"; // Маленькая картинка (водяной знак)
            $img2 = imagecreatefrompng($imgM);
        }
        else if((int)$type === 0)
        {
            $imgM = "images/site/lider.png"; // Маленькая картинка (водяной знак)
            $img2 = imagecreatefrompng($imgM);
        }
        else {
            $img2 = false;
        }
        
        if($image !== "" && $image !== null) {
            $img1 = imagecreatefromjpeg($image);
        }
        else {
            $img1 = imagecreatefrompng("images/site/noimage.png");
        }
        

        if($img1 && $img2)
        {
            $x1 = imagesx($img2);
            $y1 = imagesy($img2);

            $bw = imagesx($img1);
            $bh = imagesy($img1);

            $mtop = $bh/10;
            $mw = 3*$bw/10;

            if($pos === 1) //Если большое изображение (В подробном описании товара)
            {
                $f = fopen("bigImg.txt", "w");
                fwrite($f, "big");
                imagecopyresampled($img1, $img2, 0, 30, 0, 0, $x1, $y1, $x1, $y1);

            }
            else
            {
                $f = fopen("midImg.txt", "w");
                fwrite($f, "mid");
                imagecopyresampled($img1, $img2, 0, $mtop, 0, 0, $mw, $mtop, $x1, $y1); 

            }


        }
        
        if($pos === 5) {
            $bw = imagesx($img1);
            $bh = imagesy($img1);
            $m_w = $bw / 2;
            $m_h = $bh / 2;
            $background = imagecreatetruecolor($m_w, $m_h);
            $white = imagecolorallocate($background, 255, 255, 255);
            imagefill($background, 0, 0, $white);
            imagecopyresized($background, $img1, 0, 0, 0, 0, $m_w, $m_h, $bw, $bh);
            //imagecopyresampled($img1, $img2, 0, $mtop, 0, 0, 0, 0, $x1 / 2, $y1 / 2);
            $img1 = $background;
        }
        
        ob_start();
        imagejpeg($img1);
        $nf = ob_get_contents();
        imagedestroy($img1);
        ob_end_clean();
        return 'data:image/jpeg;base64,' . chunk_split(base64_encode($nf));
    }
    
}
