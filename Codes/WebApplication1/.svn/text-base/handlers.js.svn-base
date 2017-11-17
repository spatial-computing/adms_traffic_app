var selectionModeOn = false;

var trafficTimeout;
var trafficXML;
var speedXML;
var predictXML;
//var averageSpeed;
//var updateTime = "";
var lastUpdateTime = "";
var predictedLastUpdateTime = "";
//var jjDir= "C:/Users/Jalal/Desktop/files/";
var infoBoxVisible = false;

var speedArr = [];
var timeArr = [];

var predictedSpeedArr = [];
var predictedTimeArr = [];
//----------------------------------------------------------------------------------------
function CheckData()
{
    if (!map) return;

    try {
        // Read update time and speed
        speedXML = loadXML("AverageSpeed.xml");
        predictXML = loadXML("PredictedSpeeds.xml");
        // Parse the update time and speed information
        ParseAverageSpeed();
        ParsePredictedSpeed();
    }
    catch(err){ }
    
//    try {
//        // Check if data is updated by Stream insight
//        if (updateTime != lastUpdateTime)
//        {
//            // Add the new value to the table 
//            AddRow(updateTime, averageSpeed);
//            
//            // Update the chart
//            if (window.DOMParser) {   // Firefox
//                timeArr.push(speedXML.childNodes[0].childNodes[0].childNodes[1].textContent);
//            }
//            else {// Explorer
//                timeArr.push(speedXML.childNodes(1).childNodes(0).childNodes(1).text);
//            }
//            speedArr.push(averageSpeed);
//            UpdateChart();
//            
//            // Update time       
//            lastUpdateTime = updateTime;
//        }
//    }
//    catch(err){ }
    
    try {
        // Read the sensor data
        GetSensorData();
	}
	catch(err){ }
    
    // Re-call itself
    trafficTimeout = setTimeout("CheckData()",15000);
}


// ----------------------------------------------------------------------------------------
function GetSensorData()
{
    // Load the xml file and place the sensors onto map
    trafficXML = loadXML("HighwayTraffic.xml");
    //trafficXML = loadXML("ArterialTraffic.xml");
    // Update sensors
    UpdateSensors();
}


// ----------------------------------------------------------------------------------------
function UpdateSensors()
{
    var speed, lat, lng, ost, dir, fst, utm;

    // Delete all shapes from the map before adding new ones
    map.DeleteAllShapes();

    // Firefox
    if (window.DOMParser)
    {

        // Add sensors on the map
        for (var i = 1; i < trafficXML.childNodes[0].childNodes.length - 1; i++) {//jjkola

            speed = trafficXML.childNodes[0].childNodes[i].childNodes[0].textContent;
		    lat = trafficXML.childNodes[0].childNodes[i].childNodes[1].textContent;
		    lng = trafficXML.childNodes[0].childNodes[i].childNodes[2].textContent;
		    ost = trafficXML.childNodes[0].childNodes[i].childNodes[3].textContent;
		    dir = trafficXML.childNodes[0].childNodes[i].childNodes[4].textContent;
		    fst = trafficXML.childNodes[0].childNodes[i].childNodes[5].textContent;
		    utm = trafficXML.childNodes[0].childNodes[i].childNodes[6].textContent;

		    AddSensor(speed, lat, lng, ost, dir, fst, utm);
        }
    }
    else {

        // Add sensors on the map
        for (var i = 0; trafficXML.childNodes(1).childNodes(i); i++) {

            speed = trafficXML.childNodes(1).childNodes(i).childNodes(0).text;
		    lat = trafficXML.childNodes(1).childNodes(i).childNodes(1).text;
		    lng = trafficXML.childNodes(1).childNodes(i).childNodes(2).text;
		    ost = trafficXML.childNodes(1).childNodes(i).childNodes(3).text;
		    dir = trafficXML.childNodes(1).childNodes(i).childNodes(4).text;
		    fst = trafficXML.childNodes(1).childNodes(i).childNodes(5).text;
		    utm = trafficXML.childNodes(1).childNodes(i).childNodes(6).text;

		    AddSensor(speed, lat, lng, ost, dir, fst, utm);
        }
    }
    return true;
}


// ----------------------------------------------------------------------------------------
function ParseAverageSpeed()
{            
	var updateTime = "";
	var averageSpeed = 0;
	
    // Firefox
    if (window.DOMParser)
    {   
        // to be displayed date format is 12:50 2009/01/05
        updateTime = speedXML.childNodes[0].childNodes[0].childNodes[1].textContent
        			+ "  "
        			+ speedXML.childNodes[0].childNodes[0].childNodes[0].textContent;
		averageSpeed = speedXML.childNodes[0].childNodes[0].childNodes[2].textContent;
    }
    else // Internet explorer
    {  		    
        // to be displayed date format is 12:50 2009/01/05
        updateTime = speedXML.childNodes(1).childNodes(0).childNodes(1).text
        			+ "  "
        			+ speedXML.childNodes(1).childNodes(0).childNodes(0).text;
		averageSpeed = speedXML.childNodes(1).childNodes(0).childNodes(2).text;
    }
    
    try {
        // Check if data is updated by Stream insight
        if (updateTime != lastUpdateTime)
        {
            // Add the new value to the table 
            AddRow(updateTime, averageSpeed);
            
            // Update the chart
            if (window.DOMParser) {   // Firefox
                timeArr.push(speedXML.childNodes[0].childNodes[0].childNodes[1].textContent);
            }
            else {// Explorer
                timeArr.push(speedXML.childNodes(1).childNodes(0).childNodes(1).text);
            }
            speedArr.push(averageSpeed);
            UpdateChart();
            
            // Update time       
            lastUpdateTime = updateTime;
        }
    }
    catch(err){ }
       
    return true;        
}

function ParsePredictedSpeed()
{            
	var updateTime = "";
	var predictedSpeed = 0;

	var predictItem = predictXML.getElementsByTagName('item');

	for (var i = 0; i < predictItem.length; i++) {
	    // Firefox
	    if (window.DOMParser)
	    {   
	        // to be displayed date format is 12:50 2009/01/05
	        updateTime = predictItem[i].childNodes[1].textContent
	        			+ "  "
	        			+ predictItem[i].childNodes[0].textContent;
	        predictedSpeed = predictItem[i].childNodes[2].textContent;
	    }
	    else // Internet explorer
	    {  		    
	        // to be displayed date format is 12:50 2009/01/05
	        updateTime = predictItem[i].childNodes(1).text
	        			+ "  "
	        			+ predictItem[i].childNodes(0).text;
	        predictedSpeed = predictItem[i].childNodes(2).text;
	    }
	    
	    try {
	        // Check if data is updated by Stream insight
//	        if (updateTime != predictedLastUpdateTime)
//	        {
	            // Update the chart
	            if (window.DOMParser) {   // Firefox
	                predictedTimeArr[i] = predictItem[i].childNodes[1].textContent;
	            }
	            else {// Explorer
	            	predictedTimeArr[i] = predictItem[i].childNodes(1).text;
	            }
	            predictedSpeedArr[i] = predictedSpeed;
	            
	            // Update time       
	            predictedLastUpdateTime = updateTime;
//	        }
	    }
	    catch(err){ }
	}

	//predictedLastUpdateTime = updateTime;

	UpdatePredictedChart();
    return true;        
}

// ----------------------------------------------------------------------------------------
function AddSensor(speed, lat, lng, ost, dir, fst, utm)
{
    var dirStr;
    switch(dir)
    {
        case "0": // North
            dirStr = "North";
            lat -= 0.0015;
            lng = parseFloat(lng) + 0.006;
            break;
        case "1": // South
            dirStr = "South";
            //lng = parseFloat(lng) + 0.001
            break;
        case "2": // East
            dirStr = "East";
            lat -= 0.003;
            //lng -= 0.001;
            break;
        case "3": // West
            dirStr = "West";
            lat -= 0.001;
            break;
    }
    
    var shape = new VEShape( VEShapeType.Pushpin, new VELatLong(lat, lng) );
                
	// Set the icon
	icon = trafficIconImage(speed);
	shape.SetCustomIcon(icon);

	// Set the info box
    map.ClearInfoBoxStyles();
    var infobox = "<table class='trafficMkrTable'><tr><td>Lanes</td><td>Current\&nbsp\;Speed</td></tr><tr><td>General Lane</td><td><span class='trafficMkrSpeed'>"
    		+ speed
    		+ "</span></td></tr></table><h7>Last Updated:"
    		+ utm + "</h7></div>";

    shape.SetTitle("<h4 style='margin:0px;padding:0px;position:absolute;top:7px;display:block;font-size:12px;'>" + ost + " " + dirStr + "</h4><h5>" + fst + "</h5>");
    shape.SetDescription( infobox );
    
	// Add the shape onto map
	map.AddShape(shape);

}

// ----------------------------------------------------------------------------------------
function trafficIconImage(speed) {

    var icon;

	if (speed >= 60 )
        icon = "images/green1.ico";
    else if (speed >= 50)
        icon = "images/green2.ico";
    else if (speed >= 40)
        icon = "images/cyan1.ico";
    else if (speed < 40 ) 
        icon = "images/red1.ico";
    

    return icon;
}

// ----------------------------------------------------------------------------------------
function loadXML(url) {
    var xhrObj;
    if (window.ActiveXObject) {
        xhrObj = new ActiveXObject("Microsoft.XMLDOM");
        xhrObj.async = false;
        xhrObj.load(url);
        requestSucceed = 1;
    } else {
        if (document.implementation && document.implementation.createDocument) {
	        xhrObj = document.implementation.createDocument("", "doc", null);
	        xhrObj.async = false;
	        loaded = xhrObj.load(url);
	        requestSucceed = 1;
        }
    }
    return xhrObj;
}


// ----------------------------------------------------------------------------------------
function ShapeHandler(e)
{
	if (infoBoxVisible) {
	    map.HideInfoBox();
	    infoBoxVisible = false;
	}
	else {
		if (e.elementID != null)
	    {
	    	document.getElementById("mouseonPopup").style.visibility = "hidden";
	        var shape = map.GetShapeByID(e.elementID);
	        map.ShowInfoBox(shape);
	        infoBoxVisible = true;
	    }
	}
}

function ShapeOnHandler(e) {
	if (infoBoxVisible) {
	    map.HideInfoBox();
	    infoBoxVisible = false;
	}
	if (e.elementID != null) {
	    var shape = map.GetShapeByID(e.elementID);
	    
	    var speed = shape.GetDescription().match(/<span class=\'trafficMkrSpeed\'>[\w]+<\/span>/g);
//	    alert(speed);
	    
//	    map.ShowInfoBox(shape);
	    var popUp = document.getElementById("mouseonPopup");
	    popUp.innerHTML = "<strong>" + speed + "</stroung>&nbsp;MPH";
	    popUp.style.left = (e.clientX + 5) + "px";
	    popUp.style.top = (e.clientY - 25) + "px";
	    popUp.style.visibility = "visible";
	}
	  
	return true;
}

function ShapeOutHandler(e) {
	
	var popUp = document.getElementById("mouseonPopup");
	popUp.style.visibility = "hidden";
	return true;
}



function AddRow(info, speed)
{
    var table = document.getElementById('speedTable');
    var rowCount = table.rows.length;
    var row = table.insertRow(rowCount);

    if (rowCount % 2)
    	row.style.background = '#EEEEEE';

    var cell1 = row.insertCell(0);
    cell1.innerHTML = info;

    var cell2 = row.insertCell(1);
    cell2.innerHTML = speed;
    cell2.style.fontWeight = "bold";
    if (speed >= 60 )
        cell2.style.background = "#3FC355";
    else if(speed >= 50)
        cell2.style.background = "#A5F663";
    else if (speed >= 40)
        cell2.style.background = "#99FFFF";
    else if (speed < 40 )
        cell2.style.background = "#F23A3A";           

}

function UpdateChart() {
    var url = "http://chart.apis.google.com/chart?cht=lc&chs=250x200&chd=t:"
                + speedArr.toString() + "&chco=FF8C00&chds=0,75,0,75&chxt=x,y,x,t&chxr=1,0,75&chxp=1,20,30,40,50,60,70&chxl=0:|" ; 
    
    if (timeArr.length == 1)    // only 1 element in the array
	    url = url + timeArr[0];
	else if (timeArr.length > 1)    
	    url = url + timeArr[0] + "|" + timeArr[timeArr.length-1];
	  
	 url = url + "|1:|20|30|40|50|60|70|2:|||||||Time|3:|Speed(mph)&chls=3,1,0&chxs=2,000000,12|3,000000,12&chg=0,13.3,5,5&chm=o,FF9900,0,-1,6.0";

	document.getElementById("chartImg").src = url;
}

function UpdatePredictedChart() {
	var url = "http://chart.apis.google.com/chart?cht=lc&chs=250x200&chd=t:"
        + predictedSpeedArr.toString() + "&chco=FF8C00&chds=0,75,0,75&chxt=x,y,x,t&chxr=1,0,75&chxp=1,20,30,40,50,60,70&chxl=0:|" ; 

	if (predictedTimeArr.length == 1)    // only 1 element in the array
	url = url + predictedTimeArr[0];
	else if (predictedTimeArr.length > 1)    
	url = url + predictedTimeArr[0] + "|" + predictedTimeArr[predictedTimeArr.length-1];
	
	url = url + "|1:|20|30|40|50|60|70|2:|||||||Time|3:|Speed(mph)&chls=3,1,0&chxs=2,000000,12|3,000000,12&chg=0,13.3,5,5&chm=o,FF9900,0,-1,6.0";

	document.getElementById("predictChartImg").src = url;
}

var startLatLng = 0;
var endLatLng = 0;

var dragStarted = false;
 
var startX;
var startY;

var rect;

function startDraging(e) {
	if (rect) {
		document.getElementById("myMap").removeChild(rect);
		rect = null;
	}
	dragStarted = true;

	startX = e.mapX;
	startY = e.mapY;

	pixel = new VEPixel(startX, startY);
	startLatLng = map.PixelToLatLong(pixel);
}

function mouseMoving(e) {
	if (dragStarted) {
		var x = e.mapX;
		var y = e.mapY;
		drawRectangle(x, y);
	}
	return true;
}

function endDraging(e) {
	dragStarted = false;

	var x = e.mapX;
	var y = e.mapY;
	pixel = new VEPixel(x, y);
	endLatLng = map.PixelToLatLong(pixel);
}

function drawRectangle(x, y) {
	if (!rect)
		rect = document.createElement("div");

	var style = rect.style;
	style.width = Math.abs(x - startX) + "px";
	style.height = Math.abs(y - startY) + "px";

	style.left = ((startX >= x) ? x : startX) + "px";
	style.top = ((startY >= y) ? y : startY) + "px";

	style.filter = 'alpha(opacity=' + 50 + ')';
	style.opacity = 0.5;
	style.backgroundColor = "#EEEEEE";
	style.borderColor = "#235087";
	style.position = "absolute";
	style.borderStyle = "solid";
	style.borderWidth = 1;
	style.zIndex = 30;

	document.getElementById("myMap").appendChild(rect);
}

function selectionMode() {

	selectionModeOn = !selectionModeOn;
	
	if (document.getElementById('radio1').checked) { //navigation
	
	    if (rect) {
			document.getElementById("myMap").removeChild(rect);
			rect = null;
		}
		
		map.DetachEvent("onmousedown",startDraging);
	    map.DetachEvent("onmousemove",mouseMoving);
	    map.DetachEvent("onmouseup",endDraging);	    
	    
	    // disable statistics button
	    document.getElementById('collectStatiscticsBtn').disabled = true;
	    
	    // reset chosen area
	    startLatLng = 0;
        endLatLng = 0;
	}
	// selection
	else if (document.getElementById('radio2').checked){
	
	    map.AttachEvent("onmousedown",startDraging);
	    map.AttachEvent("onmousemove",mouseMoving);
	    map.AttachEvent("onmouseup",endDraging);    
	    
	    document.getElementById('collectStatiscticsBtn').disabled = false;
	}
}

function collectStatisctics()
{
    // check if any are is selected
    if (startLatLng == 0 || endLatLng == 0)
    {
        alert("Please Select an Area !");
        return false;
    }
    
    var res = BingMap.CollectStatistics(startLatLng.Latitude, startLatLng.Longitude, endLatLng.Latitude, endLatLng.Longitude, GetData_CallBack);    
    //var res = BingMap.CollectStatistics(startLatLng.Latitude, startLatLng.Longitude, endLatLng.Latitude, endLatLng.Longitude);    
   
   /* if( res != null )
    {
        alert(res.value);    
    }
*/
}

function stop()
{

    var res = BingMap.StopAll(Stop_CallBack);   
    
    /* Delete Table */
    var url = "http://chart.apis.google.com/chart?cht=lc&chs=250x200&chco=FF8C00&chds=0,75,0,75&chxt=x,y,x,t&chxr=1,0,75&chxp=1,20,30,40,50,60,70&chxl=0:|00:00|1:|20|30|40|50|60|70|2:|||||||Time|3:|Speed(mph)&chls=3,1,0&chxs=2,000000,12|3,000000,12&chg=0,13.3,5,5";
    document.getElementById("chartImg").src = url;
    speedArr = [];
    timeArr = [];
    
    /* Delete table */
    var table = document.getElementById('speedTable');
    var rowCount = table.rows.length;
    for (var i=1; i < rowCount; ++i)
        document.getElementById('speedTable').deleteRow(1);
    
    /* Delete prediection chart */
    var preditedUrl = "http://chart.apis.google.com/chart?cht=lc&chs=250x200&chco=FF8C00&chds=0,75,0,75&chxt=x,y,x,t&chxr=1,0,75&chxp=1,20,30,40,50,60,70&chxl=0:|00:00|1:|20|30|40|50|60|70|2:|||||||Time|3:|Speed(mph)&chls=3,1,0&chxs=2,000000,12|3,000000,12&chg=0,13.3,5,5";
    document.getElementById("predictChartImg").src = preditedUrl;
    predictedSpeedArr = [];
    predictedTimeArr = [];
}
//----------------------------------------------------------------------------------------    
function Stop_CallBack(response)
{
}
//----------------------------------------------------------------------------------------    
function GetData_CallBack(response)
{
    //alert(response.value);
}

$(function() {
	$("#rightCol").tabs();

	$("#slider").slider({
		value:0.5,
		min: 0,
		max: 1,
		step: 0.05,
		slide: function(event, ui) {
			$("#weight").val(ui.value);			
		},
        change: function(event, ui) {
            BingMap.SetWeight(ui.value, GetData_CallBack);
        }
	});
	
	$("#predictSlider").slider({
		value:1,
		min: 1,
		max: 60,
		step: 1,
		slide: function(event, ui) {
			$("#kValue").val(ui.value);
        },
        change: function(event, ui) {
            BingMap.SetK(ui.value, GetData_CallBack);
        }
	});
});
