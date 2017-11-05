import numpy as np
import random
import sys
import matplotlib.pyplot as plt
import datetime
import codecs
from sklearn.cross_validation import train_test_split

output_length = 5
BATCH_SIZE = 10
hidden_variables = 250
dropout = 0.3
input_length = 0

np.random.seed(7)
X = []
with codecs.open("D:\\projects\\PsychoTest\\Psycho.Keras.Learning\\datasetx.csv", "r",encoding='utf-8', errors='strict') as fdata:
    for line in fdata:
        lx = []
        for v, val in enumerate(line.split(';')):
            lx.append(float(val))
        input_length = len(lx)
        X.append(lx)

Y = np.zeros((len(X), output_length), dtype=np.float)
lnum = 0
with codecs.open("D:\\projects\\PsychoTest\\Psycho.Keras.Learning\\datasety.csv", "r",encoding='utf-8', errors='strict') as fdata:
    for line in fdata:
        for v, val in enumerate(line.split(';')):
            Y[lnum,v]=float(val)
        lnum += 1

X_train, X_test, y_train, y_test = train_test_split(X, Y, test_size=0.05)

from keras.models import Sequential
from keras.layers import Dense, Activation, Dropout,Embedding,Merge,LSTM
from keras.callbacks import ModelCheckpoint, Callback

print('Training')

plt.ion()
plt.show()

class LossHistory(Callback):
    losses = []
    def on_epoch_end(self, batch, logs={}):
        self.losses.append(logs.get('loss'))
        plt.clf()
        plt.title('model accuracy')
        plt.ylabel('accuracy')
        plt.xlabel('batch')
        plt.plot(self.losses)
        plt.show()
        plt.pause(0.001)

filepath = "model_{epoch:02d}-{loss:.4f}.hdf5"
checkpoint = ModelCheckpoint(filepath, monitor='loss', verbose=1, save_best_only=True, mode='min')
history = LossHistory()
callbacks_list = [checkpoint, history]

model = Sequential()
model.add(Dense(hidden_variables,input_dim=input_length, activation='sigmoid'))
model.add(Dropout(dropout))
model.add(Dense(hidden_variables, activation='tanh'))
model.add(Dropout(dropout))
model.add(Dense(output_length, activation='tanh'))

model.compile(loss='mean_absolute_percentage_error', optimizer='rmsprop', metrics=['acc'])
print(model.summary())
for iteration in range(1, 500):
    print("Started:", datetime.datetime.now().strftime("%Y-%m-%d %H:%M"))
    print('-' * 50)
    print('Iteration', iteration)
    history = model.fit(X_train, y_train, epochs=10, batch_size=BATCH_SIZE, verbose=1, callbacks=callbacks_list)
    print("Finished:", datetime.datetime.now().strftime("%Y-%m-%d %H:%M"))
    # Final evaluation of the model
    scores = model.evaluate(X_test, y_test, verbose=0)
    print("Accuracy: %.2f%%" % (scores[1]*100))
    preds = model.predict(X_test, verbose=0)
    np.savetxt("exp.csv", y_train, delimiter=";",fmt='%.2f')
    np.savetxt("pre.csv", preds, delimiter=";",fmt='%.2f')
