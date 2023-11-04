$(document).ready(function () {
    $("#showReviewsButton").click(function () {
        $(".reviewsBox").toggle(); // Veksler mellom visning og skjuling av anmeldelsesboksen
    });
});

/*(document).ready(function () {
    $("#applyFilter").click(function () {
        let selectedFilter = $("#propertyFilter").val();
        window.location.href = "/Property/Index?sortOption=" + selectedFilter;
    });
});*/

let lastScrollTop = 0; // Lagrer forrige scroll-posisjon

window.onscroll = function () {
    let currentScrollTop = window.pageYOffset || document.documentElement.scrollTop;

    // Hvis brukeren scroller opp
    if (lastScrollTop > currentScrollTop) {
        document.querySelector('header').style.top = '0'; // Vis header
    }
    // Hvis brukeren scroller ned
    else if (currentScrollTop > 50) { // Du kan justere tallet for når headeren skal forsvinne
        document.querySelector('header').style.top = '-80px'; // Gjem header
    }

    lastScrollTop = currentScrollTop; // Oppdater siste scroll-posisjon
}





