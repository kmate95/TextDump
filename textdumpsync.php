<?php
 	$savefile= 'txtdmp';
 	if (isset($_POST['filename'])) {
 		$savefile = $_POST['filename'];
 	}
    if (isset($_POST['computer']) && isset( $_POST["data"])) {
		//we have new data
		$handle = fopen($savefile, 'w');
		//fwrite($handle, $_POST['computer']);
		//fwrite($handle, '\n');
		fwrite($handle, $_POST['data']);
		fclose($handle);
    } 
    else 
    {
    	if (isset($_POST["computer"])) {
    		//have to send data
    		echo( file_get_contents($savefile));
    	}
    	else{
    		//status check
    		if (!file_exists($savefile)) {
    					$handle = fopen($savefile, 'w');
				fwrite($handle, '\n');
				fclose($handle);
				echo date(0);
    		}else {
    		 
    			echo (filemtime($savefile));
    		}
    	}
    }
?>
