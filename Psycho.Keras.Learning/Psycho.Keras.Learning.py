import numpy as np
import random
import sys
import matplotlib.pyplot as plt
import datetime
import codecs

def review_to_sentences(review):
    return review.split()

np.random.seed(7)
sentences = []
max_sent = 0
with codecs.open("D:\\projects\\PsychoTest\\Psycho.Keras.Learning\\datasetx.csv", "r",encoding='utf-8', errors='strict') as fdata:
    for line in fdata:
        sent = review_to_sentences(line)
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

print('Vectorization...')
X = np.zeros((len(sentences), max_sent), dtype=np.int)
for s, _sent in enumerate(sentences):
    for w, word in enumerate(_sent):
        X[s, w] = word_to_indices[word]

from keras.datasets import imdb
from keras.models import Sequential
from keras.layers import Dense
from keras.layers import LSTM
from keras.layers.embeddings import Embedding
from keras.preprocessing import sequence

(X_train, y_train), (X_test, y_test) = imdb.load_data(num_words=top_words)
# truncate and pad input sequences
max_review_length = 500
X_train = sequence.pad_sequences(X_train, maxlen=max_review_length)
X_test = sequence.pad_sequences(X_test, maxlen=max_review_length)
# create the model
embedding_vecor_length = 32
model = Sequential()
model.add(Embedding(top_words, embedding_vecor_length, input_length=max_review_length))
model.add(LSTM(100))
model.add(Dense(1, activation='sigmoid'))
model.compile(loss='binary_crossentropy', optimizer='adam', metrics=['accuracy'])
print(model.summary())
model.fit(X_train, y_train, nb_epoch=3, batch_size=64)
# Final evaluation of the model
scores = model.evaluate(X_test, y_test, verbose=0)
print("Accuracy: %.2f%%" % (scores[1]*100))