import moment from 'moment';

export class TimeService {

  getLookbackTicks(lookback) {
    let lookbackDate = this.getLookbackDate(lookback);
    return this.getTicks(lookbackDate.toDate());
  }

  getLookbackDate(lookback) {
    let arr = lookback.split('-', 2);
    let currentDate = new Date();
    let lookbackDate = moment(currentDate).subtract(arr[0], arr[1]);
    
    return lookbackDate;
  }


  getTicks(date) {    
    // .net ticks at the unix epoch
    const epochTicks = 621355968000000000;

    // 10000 .net ticks per millisecond
    const ticksPerMillisecond = 10000;

    return date.getTime() * ticksPerMillisecond + epochTicks;
  }
}

const timeService = new TimeService();
export default timeService;