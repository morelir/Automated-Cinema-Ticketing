// catch the model(Order) when open
var modal = document.getElementById("myModal");


// Get the <span> element that closes the modal
var span = document.getElementsByClassName("close")[0];

var ins = document.createElement("INPUT");
ins.setAttribute("type", "text", "value");
var flag = true;
var line = 0;
var chair = 0;
var count = 0;
var x;
//--------------------------------
// catch all the button that send me to order
// i need to add onClick event to button like this - onClick="open_order(this.id)".
function open_order(clicked_id,intval)
{
    modal.style.display = "block";
    let element = document.getElementById("phide")
    element.innerHTML = intval
    

}
//--------------------------------

// When the user clicks on <span> (x), close the modal
span.onclick = function() {
  modal.style.display = "none";
}

// When the user clicks anywhere outside of the modal, close it
window.onclick = function(event) {
  if (event.target == modal) {
    modal.style.display = "none";
  }
}


// switch off the button
function SwitchToOff(clicked,i,j) {
    
    var element = document.getElementById(clicked);
    if (i == line && j == chair) {
        
        if (flag==true) {
            element.className = element.className.replace(/\bbtn btn-primary\b/g, "btn btn-success");
            flag = false;
            
        }
        else {
            element.className = element.className.replace(/\bbtn btn-success\b/g, "btn btn-primary");
            flag = true;
            
        }

    }
    else {
        flag = true;
        element.className = element.className.replace(/\bbtn btn-success\b/g, "btn btn-primary");
        if (count != 0) {
            var elem = document.getElementById(x);
            elem.className = elem.className.replace(/\bbtn btn-primary\b/g, "btn btn-success");
        }
    }
    line = i;
    chair = j;
    x = clicked;
    
    count++;
    if (flag == false) {
        document.getElementById("line").value = String();
        document.getElementById("chair").value = String();
    }
    else {
        document.getElementById("line").value = String(i);
        document.getElementById("chair").value = String(j);

    }
    
}










