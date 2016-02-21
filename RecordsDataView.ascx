<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RecordsDataView.ascx.cs" Inherits="RecordsDataView" %>
<table id="dataview" class="records" runat="server"></table>

<script type="text/javascript">
    var allRadios = document.getElementsByClassName('collapserad');
    var booRadio;
    var x = 0;
    for(x = 0; x < allRadios.length; x++){

        allRadios[x].onclick = function() {
            if(booRadio == this){
                this.checked = false;
                booRadio = null;
            }else{
                booRadio = this;
            }
        };
    }

    function confirmcount() {
        var count = document.querySelectorAll('.rowcheck input[type="checkbox"]:checked').length;
        if (count == 0) {
            alert("No rows selected.");
            return false;
        } else return confirm('Are you sure you wish to delete ' + count + ' row(s)?');
    }

    function reloadAsGet() {
        var loc = window.location;
        window.location = loc.protocol + '//' + loc.host + loc.pathname + loc.search;
    }
</script>