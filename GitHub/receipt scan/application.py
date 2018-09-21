import pickle
import json
import datefinder
import re
from flask import Flask, request

# this is how we initialize a flask application
application = Flask(__name__)
# To Extract  receipt Date
def get_Receipt_Date(ocr):
    all = re.findall(r"[\d]{1,2}/[\d]{1,2}/[\d]{4}", ocr)  # dd/MM/YYYY
    if len(all) == 0:
        all = re.findall(r"[\d]{1,2}/[\d]{1,2}/[\d]{1,2}", ocr)  # dd/MM/YY
    if len(all) == 0:
        all = re.findall(r"[\d]{1,2}-[\d]{1,2}-[\d]{4}", ocr)  # dd-MM-YYYY
    if len(all) == 0:
        all = re.findall(r"[\d]{1,2}-[\d]{1,2}-[\d]{2}", ocr)  # dd-MM-YY
    if len(all) == 0:  # 10 OCT 2015  10 October 2015
        all = re.findall(r"([\d]{1,2}\s(?:JANUARY|FEBRUARY|MARCH|APRIL|MAY|JUNE|JULY|AUGUST|SEPTEMBER|OCTOBER|NOVEMBER|DECEMBER|January|February|March|April|May|June|July|August|September|October|November|December|Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Sept|Oct|Nov|Dec|jan|feb|mar|apr|may|jun|jul|aug|sep|sept|oct|nov|dec|JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|SEPT|OCT|NOV|DEC)\s[\d]{4})",ocr)
    if len(all) == 0:  # 10 OCT 15  10 October 15
        all = re.findall(r"([\d]{1,2}\s(?:JANUARY|FEBRUARY|MARCH|APRIL|MAY|JUNE|JULY|AUGUST|SEPTEMBER|OCTOBER|NOVEMBER|DECEMBER|January|February|March|April|May|June|July|August|September|October|November|December|Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Sept|Oct|Nov|Dec|jan|feb|mar|apr|may|jun|jul|aug|sep|sept|oct|nov|dec|JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|SEPT|OCT|NOV|DEC)\s[\d]{2})",ocr)
    if len(all) == 0:  # 10-OCT-2012  10-October-2015
        all = re.findall(r"([\d]{1,2}-(?:JANUARY|FEBRUARY|MARCH|APRIL|MAY|JUNE|JULY|AUGUST|SEPTEMBER|OCTOBER|NOVEMBER|DECEMBER|January|February|March|April|May|June|July|August|September|October|November|December|Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Sept|Oct|Nov|Dec|jan|feb|mar|apr|may|jun|jul|aug|sep|sept|oct|nov|dec|JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|SEPT|OCT|NOV|DEC)-[\d]{4})",ocr)
    if len(all) == 0:  # 10-OCT-15  10-October-15
        all = re.findall(r"([\d]{1,2}-(?:JANUARY|FEBRUARY|MARCH|APRIL|MAY|JUNE|JULY|AUGUST|SEPTEMBER|OCTOBER|NOVEMBER|DECEMBER|January|February|March|April|May|June|July|August|September|October|November|December|Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Sept|Oct|Nov|Dec|jan|feb|mar|apr|may|jun|jul|aug|sep|sept|oct|nov|dec|JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|SEPT|OCT|NOV|DEC)-[\d]{2})",ocr)
    if len(all) == 0:  # Jun 16' 18 JUN 16, 18
        all = re.findall(r"((?:JANUARY|FEBRUARY|MARCH|APRIL|MAY|JUNE|JULY|AUGUST|SEPTEMBER|OCTOBER|NOVEMBER|DECEMBER|January|February|March|April|May|June|July|August|September|October|November|December|Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Sept|Oct|Nov|Dec|jan|feb|mar|apr|may|jun|jul|aug|sep|sept|oct|nov|dec|JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|SEPT|OCT|NOV|DEC)[\d]{2}[',.]\s[\d]{2})",ocr)  # ['][\d]{2}\s[\d]{2}
    if len(all) == 0:  # Jun 16'18 JUN 16,18
        all = re.findall(r"((?:JANUARY|FEBRUARY|MARCH|APRIL|MAY|JUNE|JULY|AUGUST|SEPTEMBER|OCTOBER|NOVEMBER|DECEMBER|January|February|March|April|May|June|July|August|September|October|November|December|Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Sept|Oct|Nov|Dec|jan|feb|mar|apr|may|jun|jul|aug|sep|sept|oct|nov|dec|JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|SEPT|OCT|NOV|DEC)[\d]{2}[',.][\d]{2})",ocr)  # ['][\d]{2}\s[\d]{2}
    if len(all) == 0:
        all = re.findall(r"((?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Sept|Oct|Nov|Dec|jan|feb|mar|apr|may|jun|jul|aug|sep|sept|oct|nov|dec|JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|SEPT|OCT|NOV|DEC)\s[.,'][\d]{2}[.,']\s[\d]{2})",ocr)  # ['][\d]{2}\s[\d]{2}
    if len(all) == 0:
        all = re.findall(
            r"((?:Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Sept|Oct|Nov|Dec|jan|feb|mar|apr|may|jun|jul|aug|sep|sept|oct|nov|dec|JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|SEPT|OCT|NOV|DEC)[.,'][\d]{2}[.,'][\d]{2})",ocr)  # ['][\d]{2}\s[\d]{2}
    print(all)
    if len(all) != 0:
        receiptDate = all[0]
        print("receiptDate:" + receiptDate)
        matches = datefinder.find_dates(receiptDate)
        print(matches)
        matche = list(matches)
        if len(matche) == 0:
            return receiptDate
        else:
            return str(matche[0])
    else:
        return 'Invalid Date format'

# To Extract  total amount
def get_Total_Amount(ocr):
    pattern = '([^@]?total[^@][\d]{1,9}.[\d]{1,9})|([^@]?total amount[^@][\d]{1,9}.[\d]{1,9})|([^@]?total \$ [^@][\d]{1,9}.[\d]{1,9})|([^@]?amount paid[^@]{1,9}.[\d]{1,9})'
    # Pass string Here and convert it to lower case
    sendText = ocr.lower()

    # Extract the price from pattern
    findPrice = re.findall(pattern=pattern, string=sendText)
    print(findPrice)
    # Iterate and fine the price and filter out the digit
    finalPrice=''
    if len(findPrice) != 0:
     for price in findPrice[0]:
        if len(price) > 2:
            finalPrice = ''.join((ch if ch in '0123456789.-e' else '') for ch in price)
            #print('receiptTotal Amount: {}'.format(str(finalPrice)))
            finalPrice=str(finalPrice)
     return finalPrice;
    else:
     return finalPrice;

@application.route("/test", methods=["POST"])
def get_test():
    return Flask.response_class(json.dumps({
        'receiptCategory': 'welcome'
    }), mimetype=u'application/json')


@application.route("/classification", methods=["POST"])
def get_Receipt_Category():
#CODE TO CLASSIFY RECEIPT CATEGORY
    value = request.get_json();
    data = value['RawText']
    clf = pickle.load(open('test_finalized_Receipt_model1.sav', 'rb'))
    count_vect = pickle.load(open('test_countvector1.sav', 'rb'))
    test = count_vect.transform([data])
    result=clf.predict(test)
    category= result[0]

#CODE TO CLASSIFY MERCHANT CATEGORY
    clf = pickle.load(open('finalized_vendor_model.sav', 'rb'))
    count_vect = pickle.load(open('vendor_countvector.sav', 'rb'))
    m_test = count_vect.transform([data])
    m_result=clf.predict(m_test)
    merchant= m_result[0]

#CODE FOR EXTRACT DATE
    receiptDate=get_Receipt_Date(data)

#CODE FOR MAX TOTAL AMOUNT
    numbers = re.findall('\d*\.\d+',data)
    numbers=map(float,numbers)
    totalAmount1= str(max(numbers))
    totalAmount2= get_Total_Amount(data)
    #print('totalAmount'+totalAmount)
    return Flask.response_class(json.dumps({
        'receiptCategory':category,
        'maxTotalAmount1': totalAmount1,
        'totalAmount2': totalAmount2,
        'receiptDate':receiptDate,
        'merchant': merchant
    }), mimetype=u'application/json')

if __name__ == '__main__':
    #app.run(host="localhost", debug=True, use_reloader=False)
    application.debug = True
    application.run()