$(document).ready(function(){
    var isEditMode = document.location.pathname.indexOf("/EditForm.aspx") > -1;
	
    if (isEditMode)
	{
        var subTypeElementId = "PredicaInvoiceLTPaymentDate_";
		var ExpResolDate = $("input[id*='" + subTypeElementId + "']").first();
		var now = new Date();
        var dateValue = (now.getMonth() + 1) + '/' + (now.getDate()+7) + '/' + now.getFullYear();
        if (window.location.href.search(new RegExp("ContentTypeId=0x0101006667822C2C904046B11878F79EFAF7A60035D6DBDCBCBB47D8B3D9F882A2652E25", "i")) !== -1) {
            dateValue = (now.getMonth() + 2) + '/' + now.getDate() + '/' + now.getFullYear();
        }
		$(ExpResolDate).val(dateValue);
	}
	
}
);
