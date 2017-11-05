import numpy as np
import random
import sys
import matplotlib.pyplot as plt
import datetime
import codecs
from sklearn.cross_validation import train_test_split

num_features = 21
max_sent = 0
output_length = 10
vocabular_words_count = 0
BATCH_SIZE = 48
hidden_variables = 42
dropout = 0.3

np.random.seed(7)
sentences = []

with codecs.open("D:\\projects\\PsychoTest\\Psycho.Keras.Learning\\datasetx.csv", "r",encoding='utf-8', errors='strict') as fdata:
    for line in fdata:
        sent = line.split()
        sentences.append(sent)
        max_sent = max(max_sent,len(sent))

words = []
for sentence in sentences:
    words.extend(sentence)

vocabular = sorted(list(set([a for a in words])))
word_to_indices = dict((c, i+1) for i, c in enumerate(vocabular))
indices_to_word = dict((i+1, c) for i, c in enumerate(vocabular))
np.save('words_to_indices.npy', word_to_indices)
np.save('indices_to_words.npy', indices_to_word)
vocabular_words_count = len(vocabular)

print('Vectorization...')
from keras.preprocessing import sequence

sentences_x = [] #np.zeros((len(sentences), max_sent), dtype=np.int)
for s, _sent in enumerate(sentences):
    line = []
    for w, word in enumerate(_sent):
        line.append(word_to_indices[word])
    sentences_x.append(line)

X = sequence.pad_sequences(sentences_x, maxlen=max_sent)
Y = np.zeros((len(sentences), output_length), dtype=np.float)
lnum = 0
with codecs.open("D:\\projects\\PsychoTest\\Psycho.Keras.Learning\\datasety.csv", "r",encoding='utf-8', errors='strict') as fdata:
    for line in fdata:
        for v, val in enumerate(line.split(';')):
            Y[lnum,v]=float(val)
        lnum += 1

X_train, X_test, y_train, y_test = train_test_split(X, Y, test_size=0.15)

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
model.add(Embedding(input_dim=vocabular_words_count, output_dim=num_features, input_length=max_sent))
model.add(Dense(hidden_variables, activation='tanh'))
#model.add(LSTM(hidden_variables, return_sequences=True))
model.add(Dropout(dropout))
model.add(Dense(hidden_variables, activation='tanh'))
#model.add(LSTM(hidden_variables, return_sequences=False))
model.add(Dropout(dropout))
model.add(Dense(output_length, activation='tanh'))

model.compile(loss='mean_absolute_error', optimizer='rmsprop', metrics=['acc'])
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
