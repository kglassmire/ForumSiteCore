// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function ago(val) {
  val = 0 | (Date.now() - val) / 1000;
  var unit, length = {
    second: 60, minute: 60, hour: 24, day: 7, week: 4.35,
    month: 12, year: 10000
  }, result;

  for (unit in length) {
    result = val % length[unit];
    if (!(val = 0 | val / length[unit]))
      return result + ' ' + (result - 1 ? unit + 's' : unit);
  }
}

function agofromnow(v) {
  if (v > Date.now())
    return ago(2 * Date.now() - v) + ' from now';
  else
    return ago(v) + ' ago';
}

