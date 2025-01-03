//CONTRARELOJ
class CountDown {
    constructor(time, output) {
        this.counter = null;
        this.baseTime = time;
        this.leftTime = time;
        this.outputElement = output;
    }

    //INICIA EL CONTADOR
    init() {
        if (!this.counter) {
            this.counter = setInterval(() => this.updateTime(), 1000); //ACTUALIZA EL TIEMPO CADA SEGUNDO
        }
    }

    //REINICIA EL CONTADOR
    restart(newTime) {
        this.delete(); //ELIMINA EL CONTADOR ACTUAL
        this.leftTime = newTime;
        this.baseTime = newTime;
        this.init(); //INICIA NUEVO CONTADOR
    }

    //ELIMINA EL CONTADOR
    delete() {
        clearInterval(this.counter); //PAUSA EL CONTADOR
        this.counter = null; //ANULA EL VALOR DEL CONTADOR
        this.leftTime = this.baseTime; //ESTABLECE EL TIEMPO RESTANTE A SU ESTADO ORIGINAL
    }

    //ACTUALIZA EL TIEMPO RESTANTE
    updateTime() {
        this.leftTime--;
        //AL LLEGAR A CERO...
        if (this.leftTime === 0) {
            this.delete();
            this.sendEvent();
        } else {
            //ACTUALIZA EL ELEMENTO HTML
            if (this.outputElement) {
                this.outputElement.innerHTML = this.leftTime;
            }
        }
    }
    //ENVIA EL EVENTO AL ELEMENTO HTML RECIBIDO
    //PARA LLAMARLO SE DEBE ADJUNTAR CON .addEventListener('timeOver', ...)
    sendEvent() {
        const event = new CustomEvent('timeOver', { bubbles: true });
        this.outputElement.dispatchEvent(event);
    }
}