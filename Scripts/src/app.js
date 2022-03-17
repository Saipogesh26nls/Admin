var express = require('express');
var path = require('path');
var app = express();

app.set('view engine', 'ejs');
app.set('Views/NewPurchase', path.join(__dirname, 'Views/NewPurchase'));

app.get('/', function (req, res) {
    res.render('New_Purchase');
});

app.listen(3000, function () {
    console.log('ready on port 3000');
});