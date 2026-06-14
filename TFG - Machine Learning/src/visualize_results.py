import pandas as pd
import numpy as np
import os
import matplotlib.pyplot as plt
import seaborn as sns

from sklearn.compose import ColumnTransformer
from sklearn.preprocessing import StandardScaler, OneHotEncoder
from sklearn.svm import SVC
from sklearn.model_selection import cross_val_predict
from sklearn.metrics import confusion_matrix, classification_report
from sklearn.inspection import permutation_importance
from sklearn.pipeline import Pipeline

def main():
    print("="*50)
    print(" GENERANT GRÀFICS DE RESULTATS (SVM) ")
    print("="*50)

    graphs_dir = 'graphs'
    os.makedirs(graphs_dir, exist_ok=True)

    data_path = os.path.join('data', 'telemetria_entrenamiento.csv')
    df = pd.read_csv(data_path)
    X = df.drop(columns=['PerfilJugador'])
    y = df['PerfilJugador']

    numeric_features = ['APM', 'Precision', 'AvgDistance', 'DamageTakenPerMin', 
                        'DamageDealtPerMin', 'RangedRatio', 'DashPerMin', 
                        'ShieldPerMin', 'TimeInRiskZone']
    categorical_features = ['RewardChosen']

    preprocessor = ColumnTransformer(
        transformers=[
            ('num', StandardScaler(), numeric_features),
            ('cat', OneHotEncoder(), categorical_features)
        ])

    svm_model = SVC(kernel='rbf', class_weight='balanced', random_state=42)
    pipeline = Pipeline(steps=[('preprocessor', preprocessor), ('classifier', svm_model)])

    # ==========================================
    # ANÁLISIS 1: MÉTRICAS F1, PRECISION Y RECALL
    # ==========================================
    print("-> Computant prediccions amb Validació Creuada (CV=5)...")
    y_pred = cross_val_predict(pipeline, X, y, cv=5)

    print("\n" + "="*50)
    print(" INFORME DE CLASSIFICACIÓ ")
    print("="*50)
    print(classification_report(y, y_pred))
    print("="*50 + "\n")

    # ==========================================
    # ANÁLISIS 2: MATRIZ DE CONFUSIÓN
    # ==========================================
    print("-> Generant Matriu de Confusió...")
    classes = np.unique(y)
    cm = confusion_matrix(y, y_pred, labels=classes)

    plt.figure(figsize=(8, 6))
    sns.heatmap(cm, annot=True, fmt='d', cmap='Blues', xticklabels=classes, yticklabels=classes)
    plt.title('Matriu de Confusió (SVM RBF CV=5)')
    plt.ylabel('Perfil Real')
    plt.xlabel('Perfil Predit pel model')
    plt.tight_layout()
    
    cm_path = os.path.join(graphs_dir, 'confusion_matrix.png')
    plt.savefig(cm_path, dpi=300)
    plt.close()
    print(f"   [Guardat] {cm_path}")

    # ==========================================
    # ANÁLISIS 3: FEATURE IMPORTANCE
    # ==========================================
    print("-> Extraient la Importància de les Variables (Permutation Importance)...")

    pipeline.fit(X, y)

    result = permutation_importance(pipeline, X, y, n_repeats=10, random_state=42, n_jobs=-1)

    all_feature_names = X.columns

    feature_importance_df = pd.DataFrame({
        'Feature': all_feature_names,
        'Importance': result.importances_mean
    }).sort_values(by='Importance', ascending=False)

    plt.figure(figsize=(10, 8))
    sns.barplot(x='Importance', y='Feature', data=feature_importance_df, palette='viridis')
    plt.title('Importància de les Variables per al Model SVM')
    plt.xlabel('Pes en la decisió del model')
    plt.ylabel('Mètrica de Telemetria')
    plt.tight_layout()

    fi_path = os.path.join(graphs_dir, 'feature_importance.png')
    plt.savefig(fi_path, dpi=300)
    plt.close()
    print(f"   [Guardat] {fi_path}")

    print("="*50)
    print("¡Gràfics generats amb èxit! Revisa la carpeta 'graphs/'.")

if __name__ == "__main__":
    main()